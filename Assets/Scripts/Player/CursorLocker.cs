using UnityEngine;
using System.Collections;

public class CursorLocker : MonoBehaviour
{
    public network.NetworkManager networkManager;
    public delegate void CursorStateChanged(bool locked);
    public static CursorStateChanged onCursorChanged;

    private static bool cursorLocked = false;
    private static bool keyDown = false;

    public static bool isLocked()
    {
        return cursorLocked;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    } 

	void Update ()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        //if scene is offline screen. make sure we are unlocked. then return
        if(sceneName != networkManager.onlineScene)
        {
            if (cursorLocked)
                toggleLock();

            return;
        }

        //see if key state has changed. if it has, then toggle lock
        if (Input.GetButtonDown("CursorLock") && !keyDown)
        {
            toggleLock();
            keyDown = true;
        }  
        else if(Input.GetButtonUp("CursorLock"))
        {
            keyDown = false;
        }
    }

    public static void lockCursor()
    {
        if (!cursorLocked)
            toggleLock();
    }

    private static void toggleLock()
    {
        cursorLocked = !cursorLocked;

        //lock/unlock
        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //visible/not visible
        Cursor.visible = !cursorLocked;

        //notify people listening that a change occured
        if (onCursorChanged != null)
        {
            onCursorChanged(cursorLocked);
        }
    }
}
