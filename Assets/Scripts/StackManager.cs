using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StackManager : MonoBehaviour
{


    const float BOUNDS_LIMIT = 4f;
    const float STACK_MOVEMENT_SPEED = 6.0f;
    const float ERROR_MARGIN = 0.1f;
    const float STACK_BOUNDS_INCREAMENT = 0.25f;
    const int COMBO_START_INCREMENT = 5;

    public Color32[] tileColors = new Color32[4];


    GameObject[] Stack;

    int score = 0;
    int combo = 0;
    int currentStackIndex;

    Vector2 stackBounds = new Vector2(BOUNDS_LIMIT, BOUNDS_LIMIT);


    float tileTransition = 0.0f;
    float tileMoveSpeed = 3.0f;
    float secondPosition;

    bool isMovingAtX = true;
    bool isGameOver = false;
    Vector3 idealPosition; // desired Position
    Vector3 previousTilePosition; // last Tile Position

    public Text ScoreText;
    public GameObject GameOverPanel;

    void Start()
    {
        Stack = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            Stack[i] = transform.GetChild(i).gameObject;
        }

        currentStackIndex = transform.childCount - 1;
    }


    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceTile())
            {
                SpawnTile();
                score++;
                ScoreText.text = score.ToString();
            }
            else
            {
                EndGame();
            }

        }
        MoveTile();
        transform.position = Vector3.Lerp(transform.position, idealPosition, STACK_MOVEMENT_SPEED * Time.deltaTime);
    }

    void SpawnTile()
    {

        Transform t = Stack[currentStackIndex].transform;
        secondPosition = (isMovingAtX)
            ? t.localPosition.x : t.localPosition.z;

        previousTilePosition = Stack[currentStackIndex].transform.position;
        isMovingAtX = !isMovingAtX;

        currentStackIndex--;

        if (currentStackIndex < 0)
            currentStackIndex = transform.childCount - 1;

        idealPosition = (Vector3.down) * score;
        Stack[currentStackIndex].transform.localPosition = new Vector3(0, score, 0);
        Stack[currentStackIndex].transform.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

    }

    bool PlaceTile()
    {
        Transform t = Stack[currentStackIndex].transform;


        if (isMovingAtX)
        {
            float deltaX = previousTilePosition.x - t.position.x;
            if (Mathf.Abs(deltaX) > ERROR_MARGIN)
            {
                // Implement tile cutting mechanism
                combo = 0;
                stackBounds.x -= Mathf.Abs(deltaX);
                if (stackBounds.x <= 0)
                    return false;


                // Resizing the tile

                float middle_point = previousTilePosition.x + t.localPosition.x / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                CreateRubble(new Vector3(t.position.x > 0 ?
                                         t.position.x + (t.localScale.x / 2)
                                         : t.position.x - (t.localScale.x / 2), t.position.y, t.position.z),
                             new Vector3(Mathf.Abs(deltaX), 1, t.localScale.z));

                //repositioning the tile in x axis

                t.localPosition = new Vector3(middle_point - (previousTilePosition.x / 2), score, previousTilePosition.z);
            }
            else
            {
                if (combo > COMBO_START_INCREMENT)
                {
                    if (stackBounds.x > BOUNDS_LIMIT)
                        stackBounds.x = BOUNDS_LIMIT;

                    stackBounds.x += STACK_BOUNDS_INCREAMENT;
                    // Resizing the tile

                    float middle_point = previousTilePosition.x + t.localPosition.x / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                    //repositioning the tile in x axis

                    t.localPosition = new Vector3(middle_point - (previousTilePosition.x / 2), score, previousTilePosition.z);

                }

                combo++;
                t.localPosition = new Vector3(previousTilePosition.x, score, previousTilePosition.z);
            }
        }
        else
        {
            float deltaZ = previousTilePosition.z - t.position.z;
            if (Mathf.Abs(deltaZ) > ERROR_MARGIN)
            {



                // Implement tile cutting mechanism
                combo = 0;
                stackBounds.y -= Mathf.Abs(deltaZ);
                if (stackBounds.y <= 0)
                    return false;


                // Resizing the tile

                float middle_point = previousTilePosition.z + t.localPosition.z / 2;
                t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);


                CreateRubble(new Vector3(t.position.x, t.position.y,
                                         t.position.z > 0 ?
                                         t.position.z + (t.localScale.z / 2)
                                         : t.position.z - (t.localScale.z / 2)),
                             new Vector3(t.localScale.x, 1, Mathf.Abs(deltaZ)));


                //repositioning the tile in Y axis

                t.localPosition = new Vector3(previousTilePosition.x, score, middle_point - (previousTilePosition.z / 2));

            }
            else
            {
                if (combo > COMBO_START_INCREMENT)
                {
                    if (stackBounds.y > BOUNDS_LIMIT)
                        stackBounds.y = BOUNDS_LIMIT;

                    stackBounds.y += STACK_BOUNDS_INCREAMENT;
                    // Resizing the tile

                    float middle_point = previousTilePosition.z + t.localPosition.z / 2;
                    t.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                    //repositioning the tile in x axis

                    t.localPosition = new Vector3(previousTilePosition.x, score, middle_point - (previousTilePosition.z / 2));

                }
                combo++;
                t.localPosition = new Vector3(previousTilePosition.x, score, previousTilePosition.z);
            }
        }
        return true;
    }

    void MoveTile()
    {


        if (isGameOver)
            return;

        tileTransition += Time.deltaTime * tileMoveSpeed;

        if (isMovingAtX)
            Stack[currentStackIndex].transform.localPosition = new Vector3(Mathf.Sin(tileTransition) * BOUNDS_LIMIT, score, secondPosition);
        else
            Stack[currentStackIndex].transform.localPosition = new Vector3(secondPosition, score, Mathf.Sin(tileTransition) * BOUNDS_LIMIT);

    }


    Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
    {
        if (t < .33f)
            return Color.Lerp(a, b, t / .33f);
        else if (t < .66f)
            return Color.Lerp(a, b, (t - .33f) / .33f);
        else
            return Color.Lerp(a, b, (t - .66f) / .66f);

    }

    void ColorMesh(Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;
        Color32[] colors = new Color32[vertices.Length];
        float f = Mathf.Sin(score * 0.025f);

        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Lerp4(tileColors[0], tileColors[1], tileColors[2], tileColors[3], f);
        }

        mesh.colors32 = colors;
    }



    void CreateRubble(Vector3 pos, Vector3 scale){
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.AddComponent<Rigidbody>();
    }

    void EndGame(){
        
        Debug.LogError("You lost!, better luck next time");
        isGameOver = true;
        Stack[currentStackIndex].AddComponent<Rigidbody>();

        GameOverPanel.SetActive(true);
    }


    public void RestartScene(string sceneName){

        SceneManager.LoadScene(sceneName);
        GameOverPanel.SetActive(false);

    }

}
