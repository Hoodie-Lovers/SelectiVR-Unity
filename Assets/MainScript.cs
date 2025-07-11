using SFB;
using UnityEngine;
using UnityEngine.InputSystem;
using GLTFast;
using GLTFast.Logging;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using GLTFast.Schema;
using TMPro;
using System;

// 바디는 회전도 시켜주는게 좋지않을까?
// selected parts로만 커스텀모드 진입하도록해야함.

public class AnimationControllerScript : MonoBehaviour
{
    private Keyboard keyboard;
    private bool CustomMode = false; //현재 커스텀 모드가 켜져있는지 확인하는 상태


    public Transform parentTransform;
    public Transform MainTransform;

    public GameObject Body;
    public GameObject Legs;
    public GameObject Arms;
    public GameObject Eyes;
    public GameObject Mouth;

    public Canvas CustomModeUI;
    private GameObject SelectedParts;


    public Vector3 rotationSpeed = new Vector3(0, 10f, 0);

    void Start()
    {
        keyboard = Keyboard.current;
        RefreshCustomMode();

    }


    void Update()
    {
        if (CustomMode)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }


        if (keyboard.enterKey.wasPressedThisFrame) //파일 불러오기 예시
        {
            OpenFileExplorer();
        }

        if (keyboard.cKey.wasPressedThisFrame) //커스텀 모드를 키고 끄는 예시
        {
            if (!CustomMode)
            {
                CustomMode = true;
            }
            else
            {
                CustomMode = false;
                transform.rotation = Quaternion.identity;
            }
            RefreshCustomMode();
        }

        if (keyboard.xKey.wasPressedThisFrame)//커스텀 함수 예시
        {
            if (CustomMode)
            {
                CustomMaterial(ref Arms, "Red");
            }
        }

        if (keyboard.zKey.wasPressedThisFrame)//커스텀 함수 예시
        {
            if (CustomMode)
            {
                CustomParts(ref Legs, 0, 0, 0, -0.2f, -0.2f, -0.2f);
            }
        }

        if (keyboard.spaceKey.wasPressedThisFrame) //모션 변화 함수 예시
        {
            ChangingMotion(Legs, "Walk");
            ChangingMotion(Arms, "Walk");
            ChangingMotion(Body, "YES");
        }

        if (keyboard.qKey.wasPressedThisFrame) //파츠 변화 함수 예시
        {
            ChangeParts(ref Arms, "ArmParts2", new Vector3(0, 0, 0), new Vector3(0, 180, 0), new Vector3(0.7f, 0.7f, 0.7f));
        }
    }





    public void ChangingMotion(GameObject Object, string type)
    {
        Animator animator = Object.GetComponent<Animator>();
        if (!HasParameter(animator, type, AnimatorControllerParameterType.Trigger))
        {
            Debug.Log($"'{type}' is not Exist in '{Object.name}'");
        }
        animator.SetTrigger(type);
    }
    private static bool HasParameter(Animator animator, string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == type && param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }


    public void ChangeParts(
    ref GameObject oldObject,
    string newObject,
    Vector3 PrePosition = default(Vector3),
    Vector3 PreRotation = default(Vector3),
    Vector3 PreScale = default(Vector3))
    {
        if (CustomMode)
        {
            string ObjectPath = "Parts/" + newObject;
            string ControllerPath = "Controller/AC_" + newObject;
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(ControllerPath);
            if (oldObject != null)
            {
                Destroy(oldObject);
                Debug.Log("삭제완료");
            }

            GameObject _newObject = Resources.Load<GameObject>(ObjectPath);
            if (_newObject != null)
            {
                oldObject = Instantiate(_newObject, MainTransform);
                oldObject.transform.localPosition = PrePosition;
                oldObject.transform.localRotation = Quaternion.Euler(PreRotation);
                oldObject.transform.localScale = PreScale;
                oldObject.AddComponent<Animator>();
                Animator animator = oldObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = controller;
                    Debug.Log("컨트롤러 연결");
                }
                Debug.Log("프리팹 생성");
            }
        }
    }



    public void CustomParts(
        ref GameObject Object,
        float addLocationX = 0.5f,
        float addLocationY = 0.5f,
        float addLocationZ = 0.5f,
        float addScaleX = 0.5f,
        float addScaleY = 0.5f,
        float addScaleZ = 0.5f
        )
    {
        if (Object == null)
        {
            Debug.LogError("Object가 설정되어 있지 않습니다.");
            return;
        }
        else
        {
            SelectedParts = Object;
            RefreshCustomMode();

            if (CustomMode)
            {
                Object.transform.localPosition = new Vector3(
                    Object.transform.localPosition.x + addLocationX,
                    Object.transform.localPosition.y + addLocationY,
                    Object.transform.localPosition.z + addLocationZ);


                Object.transform.localScale = new Vector3(
                    Object.transform.localScale.x + addScaleX,
                    Object.transform.localScale.y + addScaleY,
                    Object.transform.localScale.z + addScaleZ);
            }
            else
            {
                Debug.Log("CustomMode is False State");
            }
        }

    }


    public void CustomMaterial(ref GameObject Object, String color)
    {
        UnityEngine.Material loadedMat = Resources.Load<UnityEngine.Material>("Material/M_" + color);
        if (loadedMat != null)
        {
            //혹시 본체가 머터리얼가지고 있으면 본체도 바꾸기
            Renderer Orend = Object.GetComponent<Renderer>();
            if (Orend != null)
            {
                Orend.material = loadedMat;
            }

            //자식들 다 가져와서 머터리얼 바꾸기
            Renderer[] renderers = Object.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer rend in renderers)
                rend.material = loadedMat;


            Debug.Log("머터리얼 변경완료");
        }
    }


    private void RefreshCustomMode()
    {
        CustomModeUI.transform.Find("CustomModeTEXT").gameObject.SetActive(CustomMode);
        if (SelectedParts != null)
        {
            CustomModeUI.transform.Find("CustomModeTEXT").transform.Find("SelectedPartsName").GetComponent<TMP_Text>().text = SelectedParts.name;
        }
        else
        {
            CustomModeUI.transform.Find("CustomModeTEXT").transform.Find("SelectedPartsName").GetComponent<TMP_Text>().text = "없음";
        }
    }







    public void OpenFileExplorer()
    {
        var extensions = new[] {
            new ExtensionFilter("GLB Files", "glb"),
            new ExtensionFilter("All Files", "*"),
        };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select GLB File", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            Debug.Log("선택한 파일 경로: " + paths[0]);
            LoadGLB(paths[0]);
        }
        else
        {
            Debug.Log("파일 선택 취소됨 또는 경로 없음");
        }
    }

    private async void LoadGLB(string path)
    {
        if (parentTransform == null)
        {
            Debug.LogError("parentTransform이 설정되어 있지 않습니다.");
            return;
        }

        var logger = new ConsoleLogger();
        var gltf = new GltfImport(logger: logger);

        bool success = await gltf.Load(path);
        if (success)
        {
            Debug.Log("GLB 파일 로드 성공, 인스턴스화 시작!");

            var instantiator = new GameObjectInstantiator(gltf, parentTransform);

            success = await gltf.InstantiateMainSceneAsync(instantiator, CancellationToken.None);

            if (success)
            {
                Debug.Log("GLB 인스턴스 생성 완료! parentTransform 하위에 부착 완료.");
                parentTransform.localPosition = new Vector3(0, 0, 0);
                parentTransform.localRotation = Quaternion.identity;
                parentTransform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogError("GLB 인스턴스 생성 실패!");
            }
        }
        else
        {
            Debug.LogError("GLB 파일 로드 실패!");
        }
    }
}
