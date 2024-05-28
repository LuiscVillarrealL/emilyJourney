using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelectionScript : MonoBehaviour
{
    public Transform highlight;
    public Transform selection;
    private RaycastHit raycastHit;

    void Update()
    {
        // Highlight
        if (highlight != null)
        {
            if (highlight.gameObject.GetComponent<Outline>() != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = false;
            }
            else
            {
                highlight.parent.GetComponent<Outline>().enabled = false;
            }
            
            highlight = null;
        }

        // Check if mouse is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if (highlight.CompareTag("Selectable"))
            {
                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    outline.OutlineColor = Color.magenta;
                    outline.OutlineWidth = 7.0f;
                }
            }
            else if (highlight.parent != null && highlight.parent.CompareTag("Selectable"))
            {
                if (highlight.parent.GetComponent<Outline>() != null)
                {
                    highlight.parent.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.parent.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    outline.OutlineColor = Color.magenta;
                    outline.OutlineWidth = 7.0f;
                }
            }
            else
            {
                highlight = null;
            }
        }

        // Selection
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    if (selection.gameObject.GetComponent<Outline>() != null)
                    {
                        selection.gameObject.GetComponent<Outline>().enabled = false;
                    }
                    else
                    {
                        selection.parent.GetComponent<Outline>().enabled = false;
                    }
                    
                }

                if (highlight.CompareTag("Selectable"))
                {
                    selection = raycastHit.transform;
                    selection.gameObject.GetComponent<Outline>().enabled = true;
                }
                else if(highlight.parent.CompareTag("Selectable"))
                {
                    selection = raycastHit.transform.parent.transform;
                    selection.gameObject.GetComponent<Outline>().enabled = true;
                }


                
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }
            }
        }
    }
}
