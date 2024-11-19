using UnityEngine;
using UnityEngine.UI;

public class NicknameItem : MonoBehaviour
{
    private Transform _owner;
    private const float Y_OFFSET = 2.5f;
    private Text _myText;

    public NicknameItem Initialize(Transform owner)
    {
        _owner = owner;
        _myText = GetComponent<Text>();
        return this;
    }

    public void UpdateText(string newString) => _myText.text = newString;

    public void UpdatePosition() => transform.position = _owner.position + Vector3.up * Y_OFFSET;
}
