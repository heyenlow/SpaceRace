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
    private bool finishedMoving = false;

    static Builder BlinkingBuilder = null;
    static float BlinkTime = 1f;

    void Start()
    {
        homeLocation = this.transform.position;
        homeRotation = this.transform.rotation;


		anim = gameObject.GetComponentInChildren<Animator>();
    }
    public void move(Coordinate c, Game g)
    {
        createDust(); //Create Dust when object moves 
        moveBuilderToNewSquare(this.gameObject, findSquare(c), g); //moves the object to the new square
        coord.x = c.x;
        coord.y = c.y;
    }

    public void moveBuilderToNewSquare(GameObject GamePiece, GameObject Square, Game g)
    {
        Coordinate coordinateOfSquare = Coordinate.stringToCoord(Square.name);
        //this next line will need to be adjusted for the height of each level object
        Vector3 heightDiff = new Vector3(0, (g.heightAtCoordinate(coordinateOfSquare)), 0);
        var newLocation = Square.transform.position + heightDiff;

        var x_diff = (Square.transform.position.x - GamePiece.transform.position.x);
        var y_diff = (Square.transform.position.y - GamePiece.transform.position.y);
        var z_diff = (Square.transform.position.z - GamePiece.transform.position.z);

        if (x_diff != 0 || z_diff != 0)
        {
            Vector3 direction = new Vector3(x_diff, 0f, z_diff);

            var newRotation = Quaternion.LookRotation(direction);
            GamePiece.transform.rotation = newRotation;
        }

        anim.SetInteger("AnimationPar", 1);
        finishedMoving = false;
        StartCoroutine(moveToNextPoint(newLocation));
    }

    private IEnumerator moveToNextPoint(Vector3 newLocation)
    {
        while (!finishedMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, newLocation, Speed * Time.deltaTime);
            //movingBuilder.transform.rotation = Quaternion.RotateTowards(movingBuilder.transform.rotation, newRotation, RotationSpeed * Time.deltaTime);

            if (VeryCloseObject(transform.position, newLocation))
            {
                transform.position = newLocation;
             
                finishedMoving = true;
                anim.SetInteger("AnimationPar", 0);
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
        transform.rotation = homeRotation;
        coord = new Coordinate();
        finishedMoving = false;
        StartCoroutine(moveToNextPoint(homeLocation));
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
        if(HighlightManager.isHighlightObj(this.gameObject) && (BlinkingBuilder == null || this == BlinkingBuilder)) Game.recieveLocationClick(coord);
        BlinkingBuilder = null;
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
