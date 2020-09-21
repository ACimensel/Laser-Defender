using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour
{
	private float lifetime = 2f;

	void Start (){
		Destroy (gameObject, lifetime);
	}
}
