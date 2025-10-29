using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

 public class AnimatedBorder : MonoBehaviour
 {
     public RectTransform borderRect; 
     public GameObject dotPrefab;
     public float speed = 1.0f;
     public int dotsPerSide = 10;

     private List<RectTransform> dots = new List<RectTransform>();
     private float[] positions; 

     void Start()
     {
         for (int i = 0; i < dotsPerSide * 4; i++)
         {
             GameObject dot = Instantiate(dotPrefab, transform);
             dots.Add(dot.GetComponent<RectTransform>());
             positions[i] = i * (1f / (dotsPerSide * 4));
         }
     }

     void Update()
     {
         for (int i = 0; i < dots.Count; i++)
         {
             positions[i] += Time.deltaTime * speed;
             if (positions[i] > 1f) positions[i] -= 1f;

             Vector2 pos = GetPositionOnRect(positions[i]);
             dots[i].anchoredPosition = pos;
         }
     }

     Vector2 GetPositionOnRect(float t)
     {
         float perimeter = 2 * (borderRect.rect.width + borderRect.rect.height);
         float current = t * perimeter;

         if (current < borderRect.rect.width) 
         {
             return new Vector2(current, borderRect.rect.height);
         }
         current -= borderRect.rect.width;
         if (current < borderRect.rect.height) 
         {
             return new Vector2(borderRect.rect.width, borderRect.rect.height - current);
         }
         current -= borderRect.rect.height;
         if (current < borderRect.rect.width) 
         {
             return new Vector2(borderRect.rect.width - current, 0);
         }
         current -= borderRect.rect.width;
         // Left side
         return new Vector2(0, current);
     }
 }
