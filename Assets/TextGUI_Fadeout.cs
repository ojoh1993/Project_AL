using UnityEngine;
using System.Collections;

public class TextGUI_Fadeout : MonoBehaviour {

	public float ScoreDelay = 0.5f;
	// Use this for initialization
	void Start () {
		StartCoroutine("DisplayScore"); 
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.y += 0.001f;
		transform.position = pos;
	}
	IEnumerator DisplayScore()
	{
		yield return new WaitForSeconds(ScoreDelay);
		
		for(float a = 1; a >= 0; a -= 0.05f)
		{
			transform.GetComponent<GUIText>().material.color = new Vector4(1, 1, 1, a);
			yield return new WaitForFixedUpdate();
		}
		
		Destroy(gameObject);
	}
}
