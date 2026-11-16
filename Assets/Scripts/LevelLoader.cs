using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using static Levels;

public class LevelLoader : MonoBehaviour
{
    public GameObject singleBrickPrefab, doubleBrickPrefab;
    public Material[] materials;

    public void Start()
    {
        //GarbageCollector.GCMode = GarbageCollector.Mode.Disabled;

        int level = Random.Range(1, 126);

        Debug.Log("Loading level " + level);

        string levelString = levels[(level - 1) * 2];

        char[] separators = { ' ', '\n' };

        string[] tokens = levelString.Split(separators);

        for (int i = 0; i < 20 * 20; i++) {
            string token = tokens[i + 2];

            //Debug.Log("token " + token);

            if (token == "0000")
            {
                continue;
            }

            int type = token[0] - 'A'; //Debug.Log("type " + type);
            int hits = token[1] - 'A';
            int bonus = token[2] - 'A';
            int width = token[3] - 'A';

            GameObject brick = Instantiate(width == 1 ? singleBrickPrefab : doubleBrickPrefab);

            brick.GetComponent<MeshRenderer>().material = materials[type];

            int col = i % 20;
            int row = i / 20;

            brick.GetComponent<Rigidbody>().position = new Vector3(-4.75f + col * 0.5f + (width == 2 ? 0.25f : 0f), 14f -row * 0.5f);

            if (type == 11)
            {
                brick.GetComponent<BreakableSurface>().enabled = false;
                brick.tag = "Metal Brick";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
