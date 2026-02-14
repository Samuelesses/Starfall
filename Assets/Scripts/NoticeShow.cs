using UnityEngine;

public class NoticeShow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(HideNotice), 5f);
        
    }

    // Update is called once per frame
    void HideNotice()
    {
        gameObject.SetActive(false);
        
    }
}
