﻿using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {
	public void LoadLevel(int _l) {
		GameController.LoadScene (_l);
	}
}