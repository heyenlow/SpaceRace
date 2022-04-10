using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public ParticleSystem dust; //Dust Effect
    public Coordinate coord = new Coordinate();
    private Vector3 homeLocation;
    private Quaternion homeRotation;
    static float Speed = 8f;
    private Animator anim;
    public bool currentlyMoving = false;
    public static Builder BlinkingBuilder = null;
    [SerializeField]
    private AudioSource Running;

    void Start()
    {
        homeLocation = this.transform.position;
        homeRotation = this.transform.rotation;

		anim = gameObject.GetComponentInChildren<Animator>();
    }
    public void move(Coordinate c, Game g)
    {
        moveBuilderToNewSquare(this.gameObject, findSquare(c), g); //moves the object to the new square
        coord.x = c.x;
        coord.y = c.y;
    }

    public void moveBuilderToNewSquare(GameObject GamePiece, GameObject Square, Game g)
    {
        Coordinate coordinateOfSquare = Coordinate.stringToCoord(Square.name);
        int levelAtCoord = g.levelAtCoord(coordinateOfSquare);
        Vector3 newLocation;

        if (levelAtCoord == 1 || levelAtCoord == 2)
        {
            newLocation = Square.transform.GetChild(0).transform.GetChild(levelAtCoord - 1).transform.position;
        }
        else
        {
            //this next line will need to be adjusted for the height of each level object
            Vector3 heightDiff = new Vector3(0, (g.heightAtCoordinate(coordinateOfSquare)), 0);
            newLocation = Square.transform.position + heightDiff;
        }
        var x_diff = (Square.transform.position.x - GamePiece.transform.position.x);
        var y_diff = (Square.transform.position.y - GamePiece.transform.position.y);
        var z_diff = (Square.transform.position.z - GamePiece.transform.position.z);

        if (x_diff != 0 || z_diff != 0)
        {
            Vector3 direction = new Vector3(x_diff, 0f, z_diff);

            var newRotation = Quaternion.LookRotation(direction);
            GamePiece.transform.rotation = newRotation;
        }
        //  Debug.Log("Moving to: " + Square.name + "  with height: " + g.heightAtCoordinate(coordinateOfSquare));
        //  Debug.Log(transform.position.x + " " + transform.position.y + " " + transform.position.z);

        // Debug.Log(newLocation.x + " " + newLocation.y + " " + newLocation.z);
        if (Coordinate.Equals(coord, new Coordinate()))
        {
            createDust(); //Create Dust when object moves 
            anim.SetBool("Run", true);
        }
        else if (g.heightAtCoordinate(coordinateOfSquare) > 2 || g.heightAtCoordinate(coord) > 2)
        {
            anim.SetBool("Jump", true);
        }
        else
        {
            createDust(); //Create Dust when object moves 
            anim.SetBool("Run", true);
        }
        Running.Play();
        StartCoroutine(moveToNextPoint(newLocation));
    }

    private IEnumerator moveToHomePoint()
    {
        currentlyMoving = true;
        while (currentlyMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, homeLocation, Speed * Time.deltaTime);
            //movingBuilder.transform.rotation = Quaternion.RotateTowards(movingBuilder.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);

            if (VeryCloseObject(transform.position, homeLocation))
            {
                transform.position = homeLocation;
                currentlyMoving = false;

                anim.SetBool("Run", false);
                anim.SetBool("Jump", false);

                dust.Stop();
            }
            yield return null;
        }
    }

    private IEnumerator moveToNextPoint(Vector3 newLocation)
    {
        currentlyMoving = true;
        while (currentlyMoving && !Game.cancelTurn)
        {
            transform.position = Vector3.MoveTowards(transform.position, newLocation, Speed * Time.deltaTime);
            //movingBuilder.transform.rotation = Quaternion.RotateTowards(movingBuilder.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);

            if (VeryCloseObject(transform.position, newLocation))
            {
                transform.position = newLocation;
                currentlyMoving = false;

                anim.SetBool("Run", false);
                anim.SetBool("Jump", false);

                dust.Stop();
            }
            yield return null;
        }
    }

    private bool VeryCloseObject(Vector3 a, Vector3 b)
    {
        float maxDiff = .01f;
        var xdiff = Mathf.Abs(a.x - b.x) < maxDiff;
        var ydiff = Mathf.Abs(a.y - b.y) < maxDiff;
        var zdiff = Mathf.Abs(a.z - b.z) < maxDiff;

        return xdiff && ydiff && zdiff;
    }

    public GameObject findSquare(Coordinate c)
    {
        return GameObject.Find(Coordinate.coordToString(c));
    }

    public Coordinate getLocation()
    {
        return coord;
    }
    public void returnHome()
    {
        removeHighlight();
        transform.rotation = homeRotation;
        coord = new Coordinate();
        StartCoroutine(moveToHomePoint());
    }

    private void OnMouseOver()
    {
        if (HighlightManager.isHighlightObj(this.gameObject))
        {
            highlight();
        }
    }

    private void OnMouseExit()
    {
        if (this == BlinkingBuilder)
        {
            Blink();
        }
        else
        {
            removeHighlight();
        }
    }

    void OnMouseDown()
    {
        //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        if (HighlightManager.isHighlightObj(this.gameObject) && (BlinkingBuilder == null || this == BlinkingBuilder))
        {
            Game.recieveLocationClick(coord);
            BlinkingBuilder = null;
        }
    }

    void createDust(){
        //set particle duration to be equal to speed of object 
        /*
        dust = GetComponent<ParticleSystem>();
        dust.Stop() //Never alter Duration while particle system is working

        var particleDuration = dust.main;
        particleDuration.duration = Speed * Time.deltaTime;
        */
        if (transform.position.y < .1)
        {
            dust.Play();
        }
    }

    public void Blink()
    {
        BlinkingBuilder = this;
        var materials = GetComponentInChildren<Renderer>().materials;
        foreach (var m in materials)
        {
            m.shader = Shader.Find("Shader Graphs/Blink");
        }
    }

    void highlight()
    {
        var materials = GetComponentInChildren<Renderer>().materials;
        foreach (var m in materials)
        {
            m.shader = Shader.Find("Ultimate 10+ Shaders/Plexus Line");
        }
    }

    void removeHighlight()
    {
        var materials = GetComponentInChildren<Renderer>().materials;
        foreach (var m in materials)
        {
            m.shader = Shader.Find("Universal Render Pipeline/Lit");
        }
    }

}
