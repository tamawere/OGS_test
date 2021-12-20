using UnityEngine;
/// <summary>
/// this class controlls the wheel movement and its events
/// </summary>
public class WheelBehaviour : MonoBehaviour
{

    private float initAngle;//initial click
    private float wheelAngle;//wheel´s initial angle

    private bool touchDevice=false;//detects if the device has touchScreen

    private bool wheelIsTouched = false;//detects if the initial click/touch is on the wheel
    
    private MenuMaster menu;//links with the class that has all the menu functions

    private float dropRotation;//wheel´s final at the end of the drag 

    [HideInInspector]
    public Quaternion dropSectionRotation;//rotation of the selected section
    // Start is called before the first frame update
    void Start()
    {
        /* /////////////////////////////I discarded this because it was not needed
        //check if we are on a desktop
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            touchDevice = false;
        }
        //if it isn't a desktop, lets see if our device is a handheld device aka a mobile device
        else if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            touchDevice = false;
        }
        */
        menu=Transform.FindObjectOfType<MenuMaster>();//initializes the menu function´s class
    }

    // Update is called once per frame
    void Update()
    {
        
        //uses touch position if touchScreen, if not, usesmouse position
        Vector2 pointerPos;
        if (!touchDevice)
        {
            pointerPos = Input.mousePosition;
        }
        else
        {
            pointerPos = Input.GetTouch(0).position;
        }
        //in the initial click, gets the mouse and wheel initial angles so the wheel can be draged smoothly
        if (checkInputState("begin"))
        {
            
            Ray ray = Camera.main.ScreenPointToRay(pointerPos);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if (hit.collider != null && hit.collider.transform.CompareTag("wheel"))
            {
                wheelIsTouched = true;//if the initial click was on the wheel 

                Vector2 direction = Camera.main.ScreenToWorldPoint(pointerPos) - transform.position;
                initAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Vector2 wheeldirection = transform.up;
                wheelAngle = Mathf.Atan2(wheeldirection.y, wheeldirection.x) * Mathf.Rad2Deg - 90;
            }
        }

        else if (checkInputState("dragging"))// mouse drag
        {
            if (wheelIsTouched)// if the initial click was over the wheel we rotate the wheel along the mouse dragging
            {
                Vector2 direction = Camera.main.ScreenToWorldPoint(pointerPos) - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - initAngle + wheelAngle;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
            }
        }
        else if (checkInputState("end"))//when the click is released we get the final position of the wheel and the nearest section to the arrow
        {
            if (wheelIsTouched==true)
            {
                dropRotation = transform.rotation.eulerAngles.z;

                menu.switchSection(checkSection());
                dropSectionRotation = sectionMagnet(checkSection());
            }
            wheelIsTouched = false;
        }
        else //when released we snap the wheel to the nearest section for a magnetic efect
        {
            if (Mathf.Abs(transform.rotation.z - dropSectionRotation.eulerAngles.z) > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, dropSectionRotation, 3f * Time.deltaTime);
            }
            //sectionMagnet();
        }
    }

    //checks the different states(begin,dragging,end) of the pointer(touch or mouse)
    bool checkInputState(string state)
    {
        if (touchDevice)//touch inputs
        {
            if (state == "begin")
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                    return true;
                else
                    return false;
            }
            else if (state == "dragging")
            {
                if (Input.touchCount > 0 &&  Input.GetTouch(0).phase == TouchPhase.Moved)
                    return true;
                else
                    return false;
            }
            else if (state == "end")
            {
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    return true;
                else
                    return false;
            }  
        }

        else//mouse inputs
        {
            if (state == "begin")
            {
                if (Input.GetMouseButtonDown(0))
                    return true;
                else
                    return false;
            }
            else if (state == "dragging")
            {
                if (Input.GetMouseButton(0))
                    return true;
                else
                    return false;
            }
            else if (state == "end")
            {
                if (Input.GetMouseButtonUp(0))
                    return true;
                else
                    return false;
            }
        }
        return false;
    }


    //at the end of the drag checks the closest section to the arrow
    MenuMaster.section checkSection()
    {
        if (menu.sectionLevel == 0)
        {

            if (dropRotation >= 288 || dropRotation <= 36)//Fusion section
            {
                return MenuMaster.section.fusion;
            }
            else if (dropRotation > 36 && dropRotation <= 108)//Discover section
            {
                return MenuMaster.section.discover;
            }
            else if (dropRotation > 108 && dropRotation <= 180)//forge section
            {
                return MenuMaster.section.forge;
            }
            else if (dropRotation > 108 && dropRotation < 288)//mix section
            {
                return MenuMaster.section.mix;
            }
        }
        else if (menu.sectionLevel >0)
        {
            if (menu.currentSection == MenuMaster.section.discover || menu.currentSection == MenuMaster.section.forge || menu.currentSection == MenuMaster.section.mix)
            {
                if (dropRotation >= 330 || dropRotation <= 30)
                {
                    menu.currentSubsection = 0;
                }
                else if (dropRotation > 30 && dropRotation <= 90)
                {
                    menu.currentSubsection = 1;
                }
                else if (dropRotation > 90 && dropRotation <= 150)
                {
                    menu.currentSubsection = 2;
                }
                else if (dropRotation > 150 && dropRotation <= 210)
                {
                    menu.currentSubsection = 3;
                }
                else if (dropRotation > 210 && dropRotation <= 270)
                {
                    menu.currentSubsection = 4;
                }
                else if (dropRotation > 270 && dropRotation < 330)
                {
                    menu.currentSubsection = 5;
                }
            }

        }
        return MenuMaster.section.subsection;
    }
    //returns the rotation of the selected section
    Quaternion sectionMagnet(MenuMaster.section selectedSection)
    {
        if (selectedSection== MenuMaster.section.fusion)//Fusion section
        {
            print("fusion");
            return Quaternion.Euler(0, 0, 0);
        }
        else if (selectedSection == MenuMaster.section.discover)//Discover section
        {
            print("discover");
            return Quaternion.Euler(0, 0, 72);
        }
        else if (selectedSection == MenuMaster.section.forge)//forge section
        {
            print("forge");
            return Quaternion.Euler(0, 0, 144);
        }
        else if (selectedSection == MenuMaster.section.mix)//mix section
        {
            print("mix");
            return Quaternion.Euler(0, 0, 216);
        }
        else if (selectedSection == MenuMaster.section.subsection)//mix section
        {
            print("subsection");
            return Quaternion.Euler(0, 0, menu.currentSubsection*60);
        }

        return Quaternion.identity;
    }

}
