using UnityEngine;

public class PuppetRope : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform attachedTransform;
        
    private void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, attachedTransform.position);
    }
}