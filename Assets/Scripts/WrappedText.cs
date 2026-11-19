// Copyright Sylvain Michel René Nowé 2026

using UnityEngine;
using System.Collections;
using TMPro;


public class WrappedText : MonoBehaviour
{
    public float angleShift;

    private TMP_Text m_TextComponent;

    void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
    }


    void Start()
    {
        Init();
    }

    public void Init()
    {
        Vector3[] vertices;
            
        float radius = 0.6f;

        //while (true)
        {
                
            m_TextComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.

            TMP_TextInfo textInfo = m_TextComponent.textInfo;
            int characterCount = textInfo.characterCount;


            for (int i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // Get the index of the mesh used by this character.
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                vertices = textInfo.meshInfo[materialIndex].vertices;

                    
                for (int v = 0; v < 4; v++)
                {
                    ref Vector3 vertex = ref vertices[vertexIndex + v];
                    float angle = vertex.x * -2.5f;
                    vertex.x = Mathf.Sin(angle + angleShift * Mathf.Deg2Rad) * radius;
                    vertex.z = Mathf.Cos(angle + angleShift * Mathf.Deg2Rad) * radius;
                }
            }

            //angleShift += 0.07f;

            // Upload the mesh with the revised information
            m_TextComponent.UpdateVertexData();

            //yield return new WaitForSeconds(0.025f);
        }
    }
}
