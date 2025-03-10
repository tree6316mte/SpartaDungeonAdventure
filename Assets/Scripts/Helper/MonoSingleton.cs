using UnityEngine;

// MonoSingle 사용 시 무조건 Scene에 배치하는 것이 좋음
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            // GameObject를 만들지 않아서 Awake가 되지 않았을 시 생성해주는 로직
            if (instance == null)
            {

                GameObject go = GameObject.Find(typeof(T).Name);
                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
                else
                {
                    instance = go.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    // Scene에 배치하면 Awake가 실행 되어 하나만 남기고 나머지는 Destory()
    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = GetComponent<T>();
        }
    }
}