using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using ULTRAKILL.Portal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ultrapain.Patches
{
    class Providence_CrossPatch
    {
        static bool Prefix(Drone __instance)
        {
            if (__instance.eid.enemyType == EnemyType.Providence)
            {
                __instance.enemy.parryable = false;
                if (!__instance.crashing && __instance.projectile.RuntimeKeyIsValid())
                {
                    if (!__instance.gameObject.activeInHierarchy)
                    {
                        return false;
                    }
                    UnityEngine.Vector3 position = __instance.transform.position;
                    UnityEngine.Vector3 forward = __instance.transform.forward;
                    UnityEngine.Vector3 position2 = position + forward;
                    UnityEngine.Quaternion quaternion = __instance.transform.rotation;
                    PhysicsCastResult physicsCastResult;
                    UnityEngine.Vector3 vector;
                    PortalTraversalV2[] array;
                    PortalPhysicsV2.ProjectThroughPortals(position, forward, default(LayerMask), out physicsCastResult, out vector, out array);
                    bool flag = false;
                    if (array.Length != 0)
                    {
                        PortalTraversalV2 portalTraversalV = array[0];
                        PortalHandle portalHandle = portalTraversalV.portalHandle;
                        Portal portalObject = portalTraversalV.portalObject;
                        if (portalObject.GetTravelFlags(portalHandle.side).HasFlag(PortalTravellerFlags.EnemyProjectile))
                        {
                            UnityEngine.Matrix4x4 travelMatrix = PortalUtils.GetTravelMatrix(array);
                            position2 = vector;
                            quaternion = travelMatrix.rotation * quaternion;
                        }
                        else
                        {
                            position2 = portalObject.GetTransform(portalHandle.side).GetPositionInFront(array[0].entrancePoint, 0.05f);
                            flag = !portalObject.passThroughNonTraversals;
                        }
                    }
                    List<Projectile> list = new List<Projectile>();
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.projectile.ToAsset(), position2, quaternion);
                    __instance.SetProjectileSettings(gameObject.GetComponent<Projectile>());
                    __instance.rb.AddForce(__instance.transform.forward * -1500f, ForceMode.Impulse);
                    Transform pbeam1 = gameObject.transform.GetChild(1);
                    Transform pbeam2 = gameObject.transform.GetChild(2);
                    Transform pbeam3 = gameObject.transform.GetChild(3);
                    Transform pbeam4 = gameObject.transform.GetChild(4);
                    Transform pbeam5 = UnityEngine.Object.Instantiate<Transform>(gameObject.transform.GetChild(1));
                    Transform pbeam6 = UnityEngine.Object.Instantiate<Transform>(gameObject.transform.GetChild(1));
                    Transform pbeam7 = UnityEngine.Object.Instantiate<Transform>(gameObject.transform.GetChild(1));
                    Transform pbeam8 = UnityEngine.Object.Instantiate<Transform>(gameObject.transform.GetChild(1));
                    pbeam5.SetParent(gameObject.transform); pbeam6.SetParent(gameObject.transform); pbeam7.SetParent(gameObject.transform); pbeam8.SetParent(gameObject.transform);
                    pbeam5.localPosition = UnityEngine.Vector3.zero; pbeam6.localPosition = UnityEngine.Vector3.zero; pbeam7.localPosition = UnityEngine.Vector3.zero; pbeam8.localPosition = UnityEngine.Vector3.zero;
                    List<int> thing = [0,45,90,135,180,225,270,315];
                    switch(UnityEngine.Random.Range(0, 2))
                    {
                        case 0:
                            thing = [0,45,90,135,180,225,270,315];
                            break;
                        case 1:
                            thing = [-10,10,80,100,170,190,260,280];
                            break;
                        case 2:
                            thing = [35, 55, 35+90, 55+90, 35+180, 50+180, 35+270, 50+270];
                            break;


                    }

                    pbeam1.localEulerAngles = new UnityEngine.Vector3(thing[0], 90, 0);
                    pbeam2.localEulerAngles = new UnityEngine.Vector3(thing[1], 90, 0);
                    pbeam3.localEulerAngles = new UnityEngine.Vector3(thing[2], 90, 0);
                    pbeam4.localEulerAngles = new UnityEngine.Vector3(thing[3], 90, 0);
                    pbeam5.localEulerAngles = new UnityEngine.Vector3(thing[4], 90, 0);
                    pbeam6.localEulerAngles = new UnityEngine.Vector3(thing[5], 90, 0);
                    pbeam7.localEulerAngles = new UnityEngine.Vector3(thing[6], 90, 0);
                    pbeam8.localEulerAngles = new UnityEngine.Vector3(thing[7], 90, 0);
                }
                return false;
            }
            return true;
        }
    }
}
