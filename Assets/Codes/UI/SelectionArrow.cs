using UnityEngine;
using UnityEngine.UI;

public class SelectionArrow : MonoBehaviour
{
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private AudioClip changeSound;
    [SerializeField] private AudioClip interactSound;
    [SerializeField] private float xStep = 300f;
    private Vector3 startPosition;
    private RectTransform arrow;
    private int currentPosition;

    private void Awake()
    {
        arrow = GetComponent<RectTransform>();
        startPosition = arrow.position;
    }
    private void OnEnable()
    {
        currentPosition = 0;
        ChangePosition(0);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            ChangePosition(-1);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            ChangePosition(1);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void ChangePosition(int _change)
    {
        currentPosition += _change;

        if (_change != 0)
        {
            SoundManager.instance.PlaySound(changeSound);
        }

        if (currentPosition < 0)
        {
            currentPosition = buttons.Length - 1;
        }
        else if (currentPosition > buttons.Length - 1)
        {
            currentPosition = 0;
        }
        AssignPosition();
    }
    private void AssignPosition()
    {
        arrow.position = new Vector3(startPosition.x + (currentPosition * xStep), startPosition.y);
    }
    private void Interact()
    {
        SoundManager.instance.PlaySound(interactSound);

        buttons[currentPosition].GetComponent<Button>().onClick.Invoke();
    }
}
