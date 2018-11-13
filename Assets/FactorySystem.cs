using UnityEngine;
using FYFY;

public class FactorySystem : FSystem
{
    private Family factory_F = FamilyManager.getFamily(
      new AllOfComponents(typeof(Factory)));

    public FactorySystem()
    {
        GameObject ballFactory = factory_F.First();
        if (ballFactory != null)
        {
            Factory fact = ballFactory.GetComponent<Factory>();
            for (int i = 0; i < fact.startNumber; i++)
            {
                CreateObject(fact.prefab);


            }
        }
    }

    private void CreateObject(GameObject prefab)
    {

        GameObject go = Object.Instantiate(prefab,
            new Vector3(Random.Range(0f, 0f), Random.Range(0f,0f)),
            Quaternion.identity);
        GameObjectManager.bind(go);

        // go.transform.position = ballFactory.transform.position;
        float m = Random.Range(1.0f, 5.0f);
        //float m = 1;
        go.GetComponent<Ball>().masse = m;
        go.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.blue, Color.green, m / 5);
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
        if (familiesUpdateCount < 500)
        {
            GameObject ballFactory = factory_F.First();

            if (ballFactory != null)
            {
                Factory factory = ballFactory.GetComponent<Factory>();
                if (factory != null)
                {
                    factory.reloadProgress += Time.deltaTime;
                    if (factory.reloadProgress >= factory.reloadTime)
                    {
                        CreateObject(factory.prefab);
                        factory.reloadProgress = 0;
                    }
                }
            }
        }
    }
}