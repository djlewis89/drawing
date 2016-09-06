using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public int Size = 1024;
    public Color CanvasColor = Color.white;

    private Texture2D canvas;
    private Color[] colours;

	void Start ()
    {
        colours = new Color[Size * Size];
        for (int i = 0; i < Size * Size; ++i) {
            colours[i] = CanvasColor;
        }

        ClearCanvas();
    }

    public void ClearCanvas()
    {
        canvas = new Texture2D(Size, Size);
        canvas.SetPixels(0, 0, Size, Size, colours);
        canvas.Apply();

        GetComponent<Renderer>().material.mainTexture = canvas;
    }
}
