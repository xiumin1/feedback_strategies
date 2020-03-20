using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRGrabbableExtended : OVRGrabbable {

    public void Init(Collider[] grab)
    {
        m_grabPoints = grab;
    }

}
