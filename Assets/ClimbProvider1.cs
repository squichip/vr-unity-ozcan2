using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbProvider : MonoBehaviour
{
    public ActionBasedController leftHand;
    public ActionBasedController rightHand;

    private CharacterController character;
    private bool leftClimbing;
    private bool rightClimbing;

    private Vector3 lastLeftPos;
    private Vector3 lastRightPos;

    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (leftClimbing)
        {
            Vector3 delta = leftHand.transform.position - lastLeftPos;
            character.Move(-delta);
            lastLeftPos = leftHand.transform.position;
        }

        if (rightClimbing)
        {
            Vector3 delta = rightHand.transform.position - lastRightPos;
            character.Move(-delta);
            lastRightPos = rightHand.transform.position;
        }
    }

    public void StartLeftClimb()
    {
        leftClimbing = true;
        lastLeftPos = leftHand.transform.position;
    }

    public void EndLeftClimb()
    {
        leftClimbing = false;
    }

    public void StartRightClimb()
    {
        rightClimbing = true;
        lastRightPos = rightHand.transform.position;
    }

    public void EndRightClimb()
    {
        rightClimbing = false;
    }
}
