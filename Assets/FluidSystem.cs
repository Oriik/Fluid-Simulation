using UnityEngine;
using FYFY;

public class FluidSystem : FSystem
{

    private Vector3 g = new Vector3(0.0f, -9.8f);

    public float alpha = 500;
    public float h = 0.3f;
    public float k = 1f;
    public float rho0 = 3f;
    public float kNear = 0.66f;

    private Family _fluidGO = FamilyManager.getFamily(
       new AllOfComponents(typeof(Ball)));

    private Family _factoryGO = FamilyManager.getFamily(
      new AllOfComponents(typeof(Factory)));

    public FluidSystem()
    {
        foreach (GameObject go in _fluidGO)
        {
            onGOEnter(go);
        }
        _fluidGO.addEntryCallback(onGOEnter);
    }

    private void onGOEnter(GameObject go)
    {
        go.GetComponent<Ball>().speed = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.0f, 8.0f));
        //go.GetComponent<Ball>().speed = Vector3.zero;
    }


    // Use this to update member variables when system pause. 
    // Advice: avoid to update your families inside this function.
    protected override void onPause(int currentFrame)
    {
    }

    // Use this to update member variables when system resume.
    // Advice: avoid to update your families inside this function.
    protected override void onResume(int currentFrame)
    {
    }

    // Use to process your families.
    protected override void onProcess(int familiesUpdateCount)
    {
        UpdateVariables();

        foreach (GameObject go in _fluidGO)
        {
            Ball ball = go.GetComponent<Ball>();
            ball.speed += Time.fixedDeltaTime * g;    // vi <- vi + delta t g  
        }

        foreach (GameObject go in _fluidGO)
        {
            Ball ball = go.GetComponent<Ball>();
            ball.lastPosition = go.transform.position;    //xi prev <- xi
            go.transform.position = ball.lastPosition + Time.fixedDeltaTime * ball.speed; // xi <- xi prev + delta t * vi

            ApplySpringDisplacements(go);
        }
        foreach (GameObject go in _fluidGO)
        {       
            DoubleDensityRelaxation(go, _fluidGO);         
            //FixPosition(go);         
          
        }
        foreach (GameObject go in _fluidGO)
        {
            Ball ball = go.GetComponent<Ball>();
             ball.speed = (ball.transform.position - ball.lastPosition) / Time.fixedDeltaTime; //vi <- (xi - xi prev)/delta t
        }
    }

    

    private void FixPosition(GameObject go)
    {
        if (go.transform.position.x < -4.0f) go.transform.position = new Vector3(-4.0f, go.transform.position.y, go.transform.position.z);
        if (go.transform.position.x > 4.0f) go.transform.position = new Vector3(4.0f, go.transform.position.y, go.transform.position.z);
        if (go.transform.position.y < -4.0f) go.transform.position = new Vector3(go.transform.position.x, -4.0f, go.transform.position.z);

    }

    private void DoubleDensityRelaxation(GameObject go, Family fluidGO)
    {
        float rho = 0; // doubleDensityRelaxation
        float rhoNear = 0;

        foreach (GameObject neighbour in _fluidGO)
        {
            if (neighbour != go)
            {
                float q = Vector3.Distance(go.transform.position, neighbour.transform.position) / h;
                if (q < 1)
                {
                    rho += Mathf.Pow(1 - q, 2);
                    rhoNear += +Mathf.Pow(1 - q, 3);
                }
            }
        }
        float P = k * (rho - rho0);
        float PNear = kNear * rhoNear;

        Vector3 dx = Vector3.zero;
        foreach (GameObject neighbour in _fluidGO)
        {
            if (neighbour != go)
            {
                float q = Vector3.Distance(go.transform.position, neighbour.transform.position) / h;
                if (q < 1)
                {
                    Vector3 D = Time.fixedDeltaTime * Time.fixedDeltaTime * (P * (1 - q) + PNear * Mathf.Pow(1 - q, 2)) * Vector3.Normalize(neighbour.transform.position - go.transform.position);
                    neighbour.transform.position += D / 2;
                    dx = dx - D / 2;
                }
            }
        }
        go.transform.position += dx;
    }

    

    private void ApplySpringDisplacements(GameObject go)
    {
        Ball ball = go.GetComponent<Ball>();
        Vector3 a = Vector3.zero;
        if (go.transform.position.x < -4.0f) a.x += (alpha / ball.masse) * (-4.0f - go.transform.position.x);
        if (go.transform.position.x > 4.0f) a.x -= (alpha / ball.masse) * (go.transform.position.x - 4.0f);
        if (go.transform.position.y < -4.0f) a.y += (alpha / ball.masse) * (-4.0f - go.transform.position.y);
        go.transform.position += Time.fixedDeltaTime * Time.fixedDeltaTime * a; //applySpringDisplacement
    }

    
    private void UpdateVariables()
    {

        GameObject f = _factoryGO.First();
        Factory fact = f.GetComponent<Factory>();

        alpha = fact.alpha;
        h = fact.h;
        k = fact.k;
        rho0 = fact.rho0;
        kNear = fact.kNear;
    }
}