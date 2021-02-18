using System.Collections;
using UnityEngine;

namespace App.Util
{
    public static class AnimationUtil
    {
        public static IEnumerator MoveOverSpeed(Transform transform, Vector3 destination, float speed)
        {
            while (Vector3.Distance(transform.position, destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
        }

        public static IEnumerator MoveOverSeconds(Transform transform, Vector3 destination, float durationSeconds)
        {
            float elapsedTime = 0;
            while (elapsedTime < durationSeconds)
            {
                var position = transform.position;
                var time = Vector3.Distance(position, destination) / (durationSeconds - elapsedTime) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(position, destination, time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        public static IEnumerator Attack(Transform transform, Vector3 destination, float speed)
        {
            Transform transform1;
            (transform1 = transform).LookAt(destination);

            // それっぽくちょっと動かす
            var origin = transform1.position;
            var center = Vector3.Lerp(origin, destination, 0.5f);

            while (Vector3.Distance(transform.position, center) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, center, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = origin;
        }
        
        public static IEnumerator Blink(float duration, Material material)
        {
            var originalColor = material.color;
            var limit = Time.time + duration;
            while (Time.time < limit)
            {
                // ちょっと duration をオーバーする場合があるけど…
                material.color = new Color(1f, 1f, 0f);
                yield return new WaitForSeconds(0.1f);
                material.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
            
            material.color = originalColor;
        }
        
        public static IEnumerator Rotate(Transform transform, Transform target, float durationSeconds)
        {
            var relativePos = target.position - transform.position;
            relativePos.y = 0;
        
            var lookRotation = Quaternion.LookRotation(relativePos);
            float elapsedTime = 0;
            while (elapsedTime < durationSeconds)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, elapsedTime / durationSeconds);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
