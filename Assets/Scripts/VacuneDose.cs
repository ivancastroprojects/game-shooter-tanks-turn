using UnityEngine;

public class VacuneDose : MonoBehaviour
{
    float x, y, z;
    Vector3 pos;

    void Start()
    {
        x = Random.Range(-110, 110);
        y = 1f;
        z = Random.Range(-110, 110);
        pos = new Vector3(x, y, z);
        transform.position = pos;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Transform>().gameObject.tag == "Player1")
        {
            other.GetComponentInParent<health_damage>().TakeHealth();
        }
        else if (other.GetComponentInParent<Transform>().gameObject.tag == "Player2")
        {
            other.GetComponentInParent<health_damage>().TakeHealth();
        }
        else if (other.GetComponentInParent<Transform>().gameObject.tag == "Computer")
        {
            other.GetComponentInParent<health_damage>().TakeHealth();
        }

        Destroy(gameObject);
    }
}
