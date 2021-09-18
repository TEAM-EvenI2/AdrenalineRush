using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUIParticle : MonoBehaviour
{
    public class ParticleStruct
    {
        public float remainLifeTime;
        public Image target;

        public Vector3 vel;
    }

    public Image uiParticlePrefabs;

    public float particlePerSecond = 1;
    public float lifeTime = 0;
    public float speed = 0;
    public float vely = 0;
    private float t = 0;

    private List<ParticleStruct> livingParticles = new List<ParticleStruct>();
    private Queue<Image> dieParticles = new Queue<Image>();

    private void Update()
    {
        if (particlePerSecond > 0)
        {
            t += Time.deltaTime;

            if (t >= 1 / particlePerSecond)
            {
                t -= 1 / particlePerSecond;

                Image image = null;
                if (dieParticles.Count > 0)
                {
                    image = dieParticles.Dequeue();
                }
                else
                {
                    image = Instantiate(uiParticlePrefabs);
                    image.transform.SetParent(transform);
                }

                image.transform.position = transform.position;
                image.transform.localScale = Vector3.one;
                image.gameObject.SetActive(true);

                Vector2 dir = new Vector2(Random.Range(-1f, 1f), vely);

                livingParticles.Add(new ParticleStruct() { remainLifeTime = lifeTime, target = image, vel = dir.normalized * speed });
            }

            for (int i = livingParticles.Count - 1; i >= 0; i--)
            {
                livingParticles[i].remainLifeTime -= Time.deltaTime;
                livingParticles[i].target.transform.localScale = Vector3.one * (livingParticles[i].remainLifeTime / lifeTime);

                livingParticles[i].target.transform.localPosition += livingParticles[i].vel * Time.deltaTime;
                livingParticles[i].vel += new Vector3(Random.Range(-1f, 1f), vely) * speed * Time.deltaTime;


                if (livingParticles[i].remainLifeTime <= 0)
                {
                    dieParticles.Enqueue(livingParticles[i].target);
                    livingParticles[i].target.gameObject.SetActive(false);
                    livingParticles.RemoveAt(i);
                }
            }
        }
        else
        {
            if (livingParticles.Count > 0)
            {
                for (int i = livingParticles.Count - 1; i >= 0; i--)
                {
                    dieParticles.Enqueue(livingParticles[i].target);
                    livingParticles[i].target.gameObject.SetActive(false);
                    livingParticles.RemoveAt(i);
                }
            }
        }
    }

}
