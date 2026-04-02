using UnityEngine;
using System.Collections;

public class Breakable_Object : MonoBehaviour
{
    [SerializeField] private float breakForceThreshold = 10f;
    [SerializeField] private GameObject fracturedVersion;
    [SerializeField] private float disableDelay = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;

        float impactForce = collision.impulse.magnitude / Time.fixedDeltaTime;

        if (impactForce >= breakForceThreshold)
        {
            Break();
        }
    }

    private void Break()
    {
        fracturedVersion.transform.position = transform.position;
        fracturedVersion.transform.rotation = transform.rotation;
        fracturedVersion.SetActive(true);

        StartCoroutine(DisableAfterDelay());
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }
}