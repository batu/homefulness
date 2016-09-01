using UnityEngine;
using System.Collections;

public class gaze : MonoBehaviour {

    // Use this for initialization
    float timeScale = 3000f;
    bool inProx = false;
    bool inSphere = false;

    AudioSource soundByte;
    AudioSource ambientSound;

    GameObject soundTrigger;

    Vector3 enterPosition;
    Quaternion enterRotation; 

    Mover moverScript;

    Vector3 smallScale;
    Vector3 largeScale;

    int pressCount = 0;
    public float scale = 10;
    public Camera cam;
    public float treshold = 100;


    // Use this for initialization
    void Start() {
        
        moverScript = GetComponent<Mover>();
        smallScale = transform.localScale;
        largeScale = smallScale * scale;
    }

    IEnumerator growUp() {
        float progress = 0;

        while (progress <= 1) {
            transform.localScale = Vector3.Lerp(smallScale, largeScale, progress);
            progress += Time.deltaTime * timeScale;
            yield return null;
        }
        transform.localScale = largeScale;
        yield return null;
    }

    IEnumerator shrink() {
        float progress = 0;

        while (progress <= 1) {
            transform.localScale = Vector3.Lerp(largeScale, smallScale, progress);
            progress += Time.deltaTime * timeScale;
            yield return null;
        }
        transform.localScale = smallScale;
        yield return null;
    }

    void playSound() {
        ambientSound.Stop();
        soundByte.Play();
    }

    void enterSphere() {
        pressCount = 0;

        enterPosition = transform.position;
        enterRotation = transform.rotation;

        RaycastHit hitInfo;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(transform.position, fwd, out hitInfo, 10)) {;
            if(hitInfo.collider.gameObject.name == "Photosphere" || hitInfo.collider.gameObject.name == "VSphere") {
                ambientSound = hitInfo.collider.gameObject.GetComponentInChildren<AudioSource>();
                ambientSound.Play();
                ambientSound.loop = true;
                soundTrigger = hitInfo.collider.gameObject.transform.FindChild("SoundTrigger").gameObject;
                soundTrigger.SetActive(true);
                soundByte = hitInfo.collider.gameObject.transform.FindChild("SoundTrigger").gameObject.GetComponentInChildren<AudioSource>();
                StartCoroutine(growUp());
                transform.position = hitInfo.collider.gameObject.transform.position;
                moverScript.speed = 0;
                inSphere = !inSphere;
            }
        }
    }

    void exitSphere() {
        ambientSound.Stop();
        soundByte.Stop();
        transform.position = enterPosition;
        transform.rotation = enterRotation;
        soundTrigger.SetActive(false);
        moverScript.speed = 1;
        inSphere = !inSphere;
        StartCoroutine(shrink());

    }

    void FixedUpdate() {
       
        if (inSphere) {
            if (Input.GetKey(KeyCode.Space)) {

                RaycastHit hitInfo;
                Vector3 fwd = transform.TransformDirection(Vector3.forward);
                if (Physics.Raycast(transform.position, fwd, out hitInfo, 10)) {
                    if (hitInfo.collider.gameObject.name == "SoundTrigger") {
                        playSound();
                    }
                } else {
                    pressCount++;
                    Debug.Log(pressCount);
                    if (pressCount > 200) {
                        exitSphere();
                    }
                }
            }
        } else {
            if (Input.GetKeyDown(KeyCode.Space)) {
                enterSphere();
            }
        }





        if (inSphere) {
            if (Input.touchCount > 0) {
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary) {
                    pressCount++;
                    if (pressCount > 200) {
                        exitSphere();
                    }
                }
            }
        } else {
            if (Input.touchCount > 0) {
                if (Input.GetTouch(0).phase == TouchPhase.Began) {
                    enterSphere();
                }
            }
        }

    }
}
