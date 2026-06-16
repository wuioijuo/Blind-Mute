using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoTarget : MonoBehaviour
{
    [Header("Through-wall echo highlight")]
    public Color echoColor = new Color(0f, 1f, 1f, 0.85f);

    private Renderer[] cachedRenderers;
    private readonly Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Material echoMaterial;
    private Coroutine revealRoutine;

    private void Awake()
    {
        cachedRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void Reveal(float seconds)
    {
        if (revealRoutine != null)
        {
            StopCoroutine(revealRoutine);
        }

        revealRoutine = StartCoroutine(RevealRoutine(seconds));
    }

    private IEnumerator RevealRoutine(float seconds)
    {
        AddEchoMaterial();
        yield return new WaitForSeconds(Mathf.Max(0.1f, seconds));
        RestoreMaterials();
        revealRoutine = null;
    }

    private void AddEchoMaterial()
    {
        if (cachedRenderers == null || cachedRenderers.Length == 0)
        {
            cachedRenderers = GetComponentsInChildren<Renderer>(true);
        }

        EnsureEchoMaterial();

        foreach (Renderer rend in cachedRenderers)
        {
            if (rend == null)
            {
                continue;
            }

            if (!originalMaterials.ContainsKey(rend))
            {
                originalMaterials[rend] = rend.sharedMaterials;
            }

            Material[] current = originalMaterials[rend];
            Material[] withEcho = new Material[current.Length + 1];

            for (int i = 0; i < current.Length; i++)
            {
                withEcho[i] = current[i];
            }

            withEcho[withEcho.Length - 1] = echoMaterial;
            rend.sharedMaterials = withEcho;
        }
    }

    private void RestoreMaterials()
    {
        foreach (KeyValuePair<Renderer, Material[]> pair in originalMaterials)
        {
            if (pair.Key != null)
            {
                pair.Key.sharedMaterials = pair.Value;
            }
        }
    }

    private void EnsureEchoMaterial()
    {
        if (echoMaterial != null)
        {
            return;
        }

        // В билде Shader.Find иногда не находит кастомный шейдер, если он не привязан к ассету.
        // Поэтому сначала грузим готовый Material из папки Resources. Так Unity гарантированно кладёт шейдер в .exe-сборку.
        Material template = Resources.Load<Material>("EchoThroughWallsMaterial");

        if (template != null)
        {
            echoMaterial = new Material(template);
        }
        else
        {
            Shader shader = Shader.Find("Custom/EchoThroughWalls");

            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            echoMaterial = new Material(shader);
        }

        echoMaterial.name = "Echo_ThroughWalls_Runtime";

        if (echoMaterial.HasProperty("_Color"))
        {
            echoMaterial.SetColor("_Color", echoColor);
        }

        echoMaterial.color = echoColor;
    }
}
