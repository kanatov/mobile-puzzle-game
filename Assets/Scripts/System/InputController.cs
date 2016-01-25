using UnityEngine;
using System.Collections;

public static class InputController
{
	static float MinSwipeLength = 5;
	static float s = 0.906f;

	static Vector2 _firstPressPos;
	static Vector2 _secondPressPos;
	static Vector2 _currentSwipe;

	static Direction swipeDirection;

	static bool IsNotSwipe ()
	{
		if (_currentSwipe.magnitude < MinSwipeLength) {
			return true;
		} else {
			return false;
		}
	}

	public static class GetCardinalDirections
	{
		public static readonly Vector2 Up = new Vector2 (0, 1);
		public static readonly Vector2 Down = new Vector2 (0, -1);
		public static readonly Vector2 UpRight = new Vector2 (1, 1);
		public static readonly Vector2 UpLeft = new Vector2 (-1, 1);
		public static readonly Vector2 DownRight = new Vector2 (1, -1);
		public static readonly Vector2 DownLeft = new Vector2 (-1, -1);
	}

	public static void DetectSwipe (Snowball _snowball)
	{
		// Touch detection
		if ( Input.touches.Length > 0 ) {
			Touch t = Input.GetTouch( 0 );

			if ( t.phase == TouchPhase.Began ) {
				_firstPressPos = new Vector2( t.position.x, t.position.y );
			}
			if ( t.phase == TouchPhase.Ended ) {
				_secondPressPos = new Vector2( t.position.x, t.position.y );
				_currentSwipe = new Vector3( _secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y );

				// Make sure it was a legit swipe, not a tap
				if (IsNotSwipe ()) {return;}
				CheckDirection();

				_snowball.Dash (swipeDirection);
			}
		}

		// Mosue detection
		if ( Input.GetMouseButtonDown( 0 ) ) {
			_firstPressPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
		}

		if ( Input.GetMouseButtonUp( 0 ) ) {
			_secondPressPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
			_currentSwipe = new Vector3( _secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y );

			// Make sure it was a legit swipe, not a tap
			if (IsNotSwipe ()) {return;}
			CheckDirection();

			_snowball.Dash (swipeDirection);
		}
	}

	static void CheckDirection ()
	{
		_currentSwipe.Normalize ();

		if (Vector2.Dot (_currentSwipe, GetCardinalDirections.Up) > s) {
			swipeDirection = Direction.Up;
			return;
		}
		if (Vector2.Dot (_currentSwipe, GetCardinalDirections.Down) > s) {
			swipeDirection = Direction.Down;
			return;
		}

		if (Vector2.Dot (_currentSwipe, GetCardinalDirections.UpRight) > s) {
			swipeDirection = Direction.UpRight;
			return;
		}

		if (Vector2.Dot (_currentSwipe, GetCardinalDirections.UpLeft) > s) {
			swipeDirection = Direction.UpLeft;
			return;
		}

		if (Vector2.Dot (_currentSwipe, GetCardinalDirections.DownLeft) > s) {
			swipeDirection = Direction.DownLeft;
			return;
		}

		if (Vector2.Dot (_currentSwipe, GetCardinalDirections.DownRight) > s) {
			swipeDirection = Direction.DownRight;
			return;
		}
	}
}