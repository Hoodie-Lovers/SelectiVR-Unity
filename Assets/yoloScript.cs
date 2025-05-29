using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationControllerScript : MonoBehaviour
{
    private Animator animator;
    private Keyboard keyboard;
    private bool check = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        keyboard = Keyboard.current;
    }

    void Update()
    {
        if (keyboard.spaceKey.wasPressedThisFrame) //유니티 테스트용
        {
            ChangingMotion();
        }

    }
    
    public void ChangingMotion()
    {
        if (check)
        {
            check = false;
            animator.SetTrigger("TriggerA");
            Debug.Log("triggered AAAAAAA!!");
        }
        else
        {
            check = true;
            animator.SetTrigger("TriggerB");
            Debug.Log("triggered BBBBBBB!!");
        }
    }
}
