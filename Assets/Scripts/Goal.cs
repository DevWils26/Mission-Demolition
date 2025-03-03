using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Goal : MonoBehaviour
{
    // A static field accessible by code anywhere
    static public bool goalMet = false;

    [Header("Goal Sound")]
    public AudioSource goalMusic; // AudioSource for background music
    public AudioClip goalMusicClip; // Background music clip

    void OnTriggerEnter(Collider other)
    {
        // When the trigger is hit by something
        // Check to see if it's a Projectile
        Projectile proj = other.GetComponent<Projectile>();
        
        if (proj != null){
            // If so, set goalMet to true
            Goal.goalMet = true;
            if (goalMusic != null && goalMusicClip != null)
            {
            goalMusic.clip = goalMusicClip; // Set the clip
            goalMusic.Play(); // Start playing the music
            }
            // Also set the alpha of the color to higher opacity
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 0.75f;
            mat.color = c;
        }
    }

}
