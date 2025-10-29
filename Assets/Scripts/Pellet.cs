using UnityEngine;

public class Pellet : MonoBehaviour
{
    public int points = 10;

    protected virtual void Eat()
    {
        Object.FindFirstObjectByType<GameManager>().PelletEaten(this);
        Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("PacStudent")) {
        Eat();
    }

    }

}

