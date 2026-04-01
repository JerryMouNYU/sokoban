using UnityEngine;

public class Visual_Wire : MonoBehaviour
{
    [SerializeField]
    GameObject WireStright;
    [SerializeField]
    GameObject Wire90;
    [SerializeField]
    GameObject WireHalf;
    [SerializeField]
    GameObject WireCross;

    [SerializeField]
    MeshRenderer[] WireLightPart;
    [SerializeField]
    Material wireDepowerMat;
    [SerializeField]
    Material wirePoweredMat;
    int previouisMask;

    public void ApplyState(bool powered, bool up, bool down, bool left, bool right)
    {
        ChangeDirectionalVisual(up, down, left, right);
        ChangeLight(powered);
    }

    void ChangeDirectionalVisual(bool up, bool down, bool left, bool right)
    {
        int mask = 0;
        if (up) mask |= 1;
        if (right) mask |= 2;
        if (down) mask |= 4;
        if (left) mask |= 8;

        if (mask == previouisMask)
        {
            return;
        }

        previouisMask = mask;

        switch (mask)
        {
            //  less than 1; Don't Change
            case 0:
            case 1:
            case 2:
            case 4:
            case 8:
                return;
            
            //  2 connect Stright
            case 5: SetStraight(false); break;
            case 10: SetStraight(true); break;
            
            //  2 connect 90
            case 9:  Set90(0); break;
            case 3:  Set90(1); break;//1+2
            case 6:  Set90(2); break;
            case 12: Set90(3); break;
            
            //   
            case 7:  SetThird(0); break; //missing left
            case 14: SetThird(1); break; // missing up
            case 13: SetThird(2); break; // missing right
            case 11: SetThird(3); break; // missing down
            
            //
            case 15: SetCross();  break; 


        }

    }

    void SetStraight(bool hori)
    {
        DisableAll();
        WireStright.SetActive(true);
        WireStright.transform.localRotation = hori ? Quaternion.Euler(-90,0 , 0) :Quaternion.Euler(-90,0 , 90);

    }
    void Set90(int rotationType)
    {
        if (rotationType < 0 || rotationType > 3) Debug.LogError("Out Of Range");
        DisableAll();
        Wire90.SetActive(true);
        Wire90.transform.localRotation = Quaternion.Euler(-90,0, rotationType * 90);

    }

    void SetThird(int rotationType)
    {
        if(rotationType<0 ||rotationType>3) Debug.LogError("Out Of Range");
        DisableAll();
        WireStright.SetActive(true);
        WireHalf.SetActive(true);
        WireStright.transform.localRotation =  Quaternion.Euler(-90,0, (rotationType) * 90);
        WireHalf.transform.localRotation = Quaternion.Euler(-90,0, (rotationType+1) * 90);
    }

    void SetCross()
    {
        DisableAll();
        WireStright.SetActive(true);
        WireCross.SetActive(true);
    }
    void DisableAll()
    {
        Wire90.SetActive(false);
        WireStright.SetActive(false);
        WireCross.SetActive(false);
        WireHalf.SetActive(false);
    }

    public void ChangeLight(bool turnOn)
    {
        if (turnOn)
        {
            foreach (MeshRenderer m in WireLightPart)
            {
                if (m != null && wirePoweredMat != null) m.sharedMaterial = wirePoweredMat;
            }
        }
        else
        {
            foreach (MeshRenderer m in WireLightPart)
            {
                if (m != null && wireDepowerMat != null) m.sharedMaterial = wireDepowerMat;
            }
        }
    }

}
