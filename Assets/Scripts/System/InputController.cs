using UnityEngine;
using System.Collections;

public enum SwipeDirection { None = -1, Up = 0, Down = 3, Left = 99, Right = 98, UpLeft = 5, UpRight = 1, DownLeft = 4, DownRight = 2 };

public static class InputController {
	static float MinSwipeLength = 5;
	static Vector2 _firstPressPos;
	static Vector2 _secondPressPos;
	static Vector2 _currentSwipe;

	static SwipeDirection swipeDirection;

	public static class GetCardinalDirections {
		public static readonly Vector2 Up = new Vector2( 0, 1 );
		public static readonly Vector2 Down = new Vector2( 0, -1 );
		public static readonly Vector2 Right = new Vector2( 1, 0 );
		public static readonly Vector2 Left = new Vector2( -1, 0 );

		public static readonly Vector2 UpRight = new Vector2( 1, 1 );
		public static readonly Vector2 UpLeft = new Vector2( -1, 1 );
		public static readonly Vector2 DownRight = new Vector2( 1, -1 );
		public static readonly Vector2 DownLeft = new Vector2( -1, -1 );
	}

	public static void DetectSwipe() {
		if ( Input.touches.Length > 0 ) {
			Touch t = Input.GetTouch( 0 );

			if ( t.phase == TouchPhase.Began ) {
				_firstPressPos = new Vector2( t.position.x, t.position.y );
			}

			if ( t.phase == TouchPhase.Ended ) {
				_secondPressPos = new Vector2( t.position.x, t.position.y );
				_currentSwipe = new Vector3( _secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y );


				// Make sure it was a legit swipe, not a tap
				if ( _currentSwipe.magnitude < MinSwipeLength ) {
					swipeDirection = SwipeDirection.None;
//					Units.Behaviour (SwipeDirection.Up);
					return;
				}

				_currentSwipe.Normalize();
				CheckDirection(_currentSwipe);
			}
		} else {
			if ( Input.GetMouseButtonDown( 0 ) ) {
				_firstPressPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
			} else {
				swipeDirection = SwipeDirection.None;
			}
			if ( Input.GetMouseButtonUp( 0 ) ) {
				_secondPressPos = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
				_currentSwipe = new Vector3( _secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y );

				// Make sure it was a legit swipe, not a tap
				if ( _currentSwipe.magnitude < MinSwipeLength ) {
					swipeDirection = SwipeDirection.None;
//					Units.Behaviour (SwipeDirection.Up);
					return;
				}

				_currentSwipe.Normalize();
				CheckDirection(_currentSwipe);
			}
		}
	}

	static void CheckDirection(Vector2 _currentSwipe) {
		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.Up ) > 0.906f ) {
			swipeDirection = SwipeDirection.Up;
//			Units.Behaviour (SwipeDirection);
			return;
		}
		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.Down ) > 0.906f ) {
			//			SwipeDirection = SwipeDirection.Down;
			//			return;
		}
		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.Left ) > 0.906f ) {
			swipeDirection = SwipeDirection.Left;
			MapController.RotateCamera (swipeDirection);
			return;
		}
		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.Right ) > 0.906f) {
			swipeDirection = SwipeDirection.Right;
			MapController.RotateCamera (swipeDirection);
			return;
		}

		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.UpRight ) >0.906f ) {
			swipeDirection = SwipeDirection.Right;
			MapController.RotateCamera (swipeDirection);
			return;
		}

		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.UpLeft ) > 0.906f ) {
			swipeDirection = SwipeDirection.Left;
			MapController.RotateCamera (swipeDirection);
			return;
		}

		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.DownLeft ) > 0.906f ) {
			//			SwipeDirection = SwipeDirection.DownLeft;
			swipeDirection = SwipeDirection.Left;
			MapController.RotateCamera (swipeDirection);
			return;
		}

		if ( Vector2.Dot( _currentSwipe, GetCardinalDirections.DownRight ) > 0.906f) {
			//			SwipeDirection = SwipeDirection.DownRight;
			swipeDirection = SwipeDirection.Right;
			MapController.RotateCamera (swipeDirection);
			return;
		}
	}
}