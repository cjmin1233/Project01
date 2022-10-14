using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float floatingSpeed;
    public float alphaSpeed;
    TextMeshPro text;
    Color alpha;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        text.text = damage.ToString();
        alpha = text.color;
        Invoke("DestroyObject", 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, floatingSpeed * Time.deltaTime, 0));
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        text.color = alpha;
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
