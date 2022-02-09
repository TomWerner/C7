
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using C7GameData;
using C7Engine;

public class AnimationTracker {
	private Civ3AnimData civ3AnimData;

	public AnimationTracker(Civ3AnimData civ3AnimData)
	{
		this.civ3AnimData = civ3AnimData;
	}

	public struct ActiveAnimation {
		public long startTimeMS, endTimeMS;
		public MapUnit.AnimatedAction action;
		public AutoResetEvent completionEvent;
	}

	private Dictionary<string, ActiveAnimation> activeAnims = new Dictionary<string, ActiveAnimation>();

	public long getCurrentTimeMS()
	{
		return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
	}

	public void startAnimation(MapUnit unit, MapUnit.AnimatedAction action, AutoResetEvent completionEvent)
	{
		Civ3Anim anim = civ3AnimData.forUnit(unit.unitType.name, action);
		long currentTimeMS = getCurrentTimeMS();
		long animDurationMS = (long)(1000.0 * anim.getDuration());

		ActiveAnimation aa;
		if (activeAnims.TryGetValue(unit.guid, out aa)) {
			// If there's already an animation playing for this unit, end it first before replacing it
			// TODO: Consider instead queueing up the new animation until after the first one is completed
			if (aa.completionEvent != null)
				aa.completionEvent.Set();
		}
		aa = new ActiveAnimation { startTimeMS = currentTimeMS, endTimeMS = currentTimeMS + animDurationMS, action = action, completionEvent = completionEvent };

		anim.playSound();

		activeAnims[unit.guid] = aa;
	}

	public void startAnimation(Tile tile, AnimatedEffect effect, AutoResetEvent completionEvent)
	{
		// TODO: Implement me
	}

	public void endAnimation(MapUnit unit)
	{
		ActiveAnimation aa;
		if (activeAnims.TryGetValue(unit.guid, out aa)) {
			if (aa.completionEvent != null)
				aa.completionEvent.Set();
			activeAnims.Remove(unit.guid);
		}
	}

	public bool hasCurrentAction(MapUnit unit)
	{
		return activeAnims.ContainsKey(unit.guid);
	}

	public (MapUnit.AnimatedAction, double) getCurrentActionAndRepetitionCount(MapUnit unit)
	{
		ActiveAnimation aa = activeAnims[unit.guid];
		var durationMS = (double)(aa.endTimeMS - aa.startTimeMS);
		if (durationMS <= 0.0)
			durationMS = 1.0;
		var repCount = (double)(getCurrentTimeMS() - aa.startTimeMS) / durationMS;
		return (aa.action, repCount);
	}

	public void update()
	{
		long currentTimeMS = getCurrentTimeMS();
		var keysToRemove = new List<string>();
		foreach (var guidAAPair in activeAnims.Where(guidAAPair => guidAAPair.Value.endTimeMS <= currentTimeMS)) {
			var (unitGUID, aa) = (guidAAPair.Key, guidAAPair.Value);
			if (aa.completionEvent != null)
				aa.completionEvent.Set();
			keysToRemove.Add(unitGUID);
		}
		foreach (var key in keysToRemove)
			activeAnims.Remove(key);
	}

	public MapUnit.Appearance getUnitAppearance(MapUnit unit)
	{
		if (hasCurrentAction(unit)) {
			var (action, repCount) = getCurrentActionAndRepetitionCount(unit);

			var isNonRepeatingAction =
				(action == MapUnit.AnimatedAction.RUN) ||
				(action == MapUnit.AnimatedAction.DEATH) ||
				(action == MapUnit.AnimatedAction.FORTIFY) ||
				(action == MapUnit.AnimatedAction.VICTORY) ||
				(action == MapUnit.AnimatedAction.BUILD);

			float progress;
			if (isNonRepeatingAction)
				progress = (repCount <= 1.0) ? (float)repCount : 1f;
			else
				progress = (float)(repCount - Math.Floor(repCount));

			float offsetX = 0, offsetY = 0;
			if (action == MapUnit.AnimatedAction.RUN) {
				(int dX, int dY) = unit.facingDirection.toCoordDiff();
				offsetX = -1 * dX * (1f - progress);
				offsetY = -1 * dY * (1f - progress);
			}

			return new MapUnit.Appearance {
				action = action,
				direction = unit.facingDirection,
				progress = progress,
				offsetX = offsetX,
				offsetY = offsetY
				};
		} else
			return new MapUnit.Appearance {
				action = unit.isFortified ? MapUnit.AnimatedAction.FORTIFY : MapUnit.AnimatedAction.DEFAULT,
				direction = unit.facingDirection,
				progress = 1f,
				offsetX = 0f,
				offsetY = 0f
				};
	}
}
