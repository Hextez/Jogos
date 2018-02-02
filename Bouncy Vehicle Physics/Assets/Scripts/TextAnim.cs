using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnim : MonoBehaviour {

    void Start()
    {
        
    }

    IEnumerator GrowTitleFont()
    {
        double fontsize = 0;
        Image im = gameObject.GetComponentInChildren<Image>();
        Text t = gameObject.GetComponentInChildren<Text>();
        while (true)
        {
            yield return new WaitForSeconds(0.05f);
            Color s = im.color;
            Color s2 = t.color;
            s.a += 0.1f;
            s2.a += 0.1f;
            Debug.Log(s.a);
            if (s.a >= 1)
                s.a = 0.0f;
            if (s2.a >= 1)
                s2.a = 0.0f;
            im.color = s;
            t.color = s2;
        }
    }
    private void OnEnable()
    {
        StartCoroutine("GrowTitleFont");
    }
}
