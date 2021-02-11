using System.Collections;
using UnityEngine;

namespace App.Scenes.Game
{
    /// <summary>
    /// アニメーション系の処理をここに分離しただけ.
    /// 後で適切なものに置き換えたい
    /// </summary>
    public class UnitAnimation : MonoBehaviour
    {
        public IEnumerator MoveOverSpeed(Vector3 destination, float speed)
        {
            transform.LookAt(destination);

            while (Vector3.Distance(transform.position, destination) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
                yield return null;
            }
        }

        public IEnumerator MoveOverSeconds(Vector3 destination, float seconds)
        {
            float elapsedTime = 0;
            while (elapsedTime < seconds)
            {
                var position = transform.position;
                var time = Vector3.Distance(position, destination) / (seconds - elapsedTime) * Time.deltaTime;
                transform.position = Vector3.MoveTowards(position, destination, time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        public IEnumerator Attack(Vector3 destination, float speed)
        {
            Transform transform1;
            (transform1 = transform).LookAt(destination);

            var origin = transform1.position;
            var center = Vector3.Lerp(origin, destination, 0.5f);

            while (Vector3.Distance(transform.position, center) > float.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, center, speed * Time.deltaTime);
                yield return null;
            }

            transform.position = origin;
        }
        
        public IEnumerator Blink(float duration, Material material)
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
    }
}
