using System.Collections.Generic;
using UnityEngine;

public class EchoAbility : MonoBehaviour
{
    [Header("Echo settings")]
    public float radius = 35f;
    public float revealTime = 4f;
    public float pulseTime = 4f; // kept for compatibility with older builders
    public float cooldown = 1.2f;
    public KeyCode echoKey = KeyCode.Space;

    [Header("Optional UI reference")]
    public UIManager uiManager; // kept for BackroomsLevelBuilder compatibility

    private float lastEchoTime = -999f;

    private void Update()
    {
        if (Input.GetKeyDown(echoKey))
        {
            TryEcho();
        }
    }

    public void TryEcho()
    {
        if (Time.time < lastEchoTime + cooldown)
        {
            return;
        }

        lastEchoTime = Time.time;
        float activeTime = Mathf.Max(revealTime, pulseTime, 0.1f);

        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        HashSet<EchoTarget> revealedTargets = new HashSet<EchoTarget>();

        foreach (Collider hit in hits)
        {
            EchoTarget target = hit.GetComponent<EchoTarget>();

            if (target == null)
            {
                target = hit.GetComponentInParent<EchoTarget>();
            }

            if (target == null)
            {
                target = hit.GetComponentInChildren<EchoTarget>();
            }

            if (target != null && !revealedTargets.Contains(target))
            {
                target.Reveal(activeTime);
                revealedTargets.Add(target);
            }
        }

        if (uiManager != null)
        {
            if (revealedTargets.Count > 0)
            {
                uiManager.ShowTemporaryMessage("Эхо обнаружило объектов: " + revealedTargets.Count, 1.3f);
            }
            else
            {
                uiManager.ShowTemporaryMessage("Эхо ничего не обнаружило рядом", 1.3f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
