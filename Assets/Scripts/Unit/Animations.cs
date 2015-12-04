using UnityEngine;
using System.Collections;

public static class Animations {

	public static void Size(Size _size, Transform _trans, Vector3 _target, float _speed) {
		if (_trans.localScale != _target) {
			_trans.localScale = Vector3.MoveTowards (
				_trans.localScale,
				_target,
				Time.deltaTime * Map.hexSpeed
				);
		} else {
			_size.enabled = false;
			GameController.AnimationsCounter = -1;

			if (Map.hexSmallScale != _trans.localScale) {
				return;
			}

			GameObject.Destroy (_size.gameObject);
		}
	}

	public static void Move(Move _move, Transform _trans, Vector3 _target, float _speed) {
		if (_trans.localPosition != _target) {
			_trans.localPosition = Vector3.MoveTowards (
				_trans.localPosition,
				_target,
				Time.deltaTime * _speed
				);
		} else {
			_move.enabled = false;
			GameController.AnimationsCounter = -1;
		}
	}
}
