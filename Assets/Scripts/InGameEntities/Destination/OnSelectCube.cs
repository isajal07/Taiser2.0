using UnityEngine;
using UnityEngine.EventSystems;

public class OnSelectCube : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.parent.GetComponentInChildren<Destination>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Destination destination;
    public void OnPointerClick(PointerEventData eventData)
    {
        RuleSpecButtonManager.inst.OnAttackableDestinationClicked(destination);
    }
   
}
