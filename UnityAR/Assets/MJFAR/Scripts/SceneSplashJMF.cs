using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Management;
using UnityEngine.XR.ARSubsystems;
//using UnityEngine.XR.ARFoundation;

namespace UnityEngine.XR.ARFoundation
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(ARUpdateOrder.k_Session)]
    public class SceneSplashJMF : SubsystemLifecycleManager<XRSessionSubsystem, XRSessionSubsystemDescriptor, XRSessionSubsystem.Provider>
    {
        int ARStatus = 0;
        //0 nada; 1 Not Supported; 2 Try Install; 3 OK

        public Text Txt1;
        public Text Txt2;
        public Text Txt3;
        public Text Txt4;
        public Text Txt5;
        public Text Txt6;
        private static ARSessionState state;
        void Start()
        {   VerificaAR();}
        private void VerificaAR()
        {   EnsureSubsystemInstanceSet();
            if (subsystem != null)
            {   StartCoroutine(Initialize());}
            else { ARStatus = 1; }
        }
        IEnumerator Initialize()
        {
            if (state <= ARSessionState.CheckingAvailability)
                yield return CheckAvailability();
            if (state == ARSessionState.Unsupported)
            {   ARStatus = 1;}
            if ((state == ARSessionState.NeedsInstall)|| (state == ARSessionState.Installing))
            {
                ARStatus = 2;
            }
            if (state == ARSessionState.Ready && enabled)
            {   ARStatus = 3;}
        }
        public void AcaoInstall()
        {
            StartCoroutine(Install());
        }
        public IEnumerator CheckAvailability()
        {
            while (state == ARSessionState.CheckingAvailability)
            {
                yield return null;
            }
            if (state != ARSessionState.None)
                yield break;
            var subsystem = GetSubsystem();
            if (subsystem == null)
            {
                state = ARSessionState.Unsupported;
            }
            else if (state == ARSessionState.None)
            {
                Txt3.text = "check";
                state = ARSessionState.CheckingAvailability;
                var availabilityPromise = subsystem.GetAvailabilityAsync();
                yield return availabilityPromise;
                Txt4.text = "promisse";

                s_Availability = availabilityPromise.result;
                if (s_Availability.IsSupported() && s_Availability.IsInstalled())
                {
                    state = ARSessionState.Ready;
                }
                else if (s_Availability.IsSupported() && !s_Availability.IsInstalled())
                {
                    Txt5.text = "install";
                    bool supportsInstall =
                        subsystem.subsystemDescriptor.supportsInstall;
                    state = supportsInstall ? ARSessionState.NeedsInstall : ARSessionState.Unsupported;
                }
                else
                {
                    Txt6.text = "unsupp";

                    state = ARSessionState.Unsupported;
                }
            }
        }

        private void Update()
        {
            switch (ARStatus)
            {
                case 0:
                    Txt1.text = "Start";
                    break;
                case 1:
                    Txt1.text = "Not Supported";
                    break;
                case 2:
                    Txt2.text = "Try install";
                    break;
                case 3:
                    Txt2.text = "AR OK";
                    break;


                    //0 nada; 1 Not Supported; 2 Try Install; 3 OK

            }
        }

        public IEnumerator Install()
        {
            while ((state == ARSessionState.Installing) || (state == ARSessionState.CheckingAvailability))
            {
                yield return null;
            }
            switch (state)
            {
                case ARSessionState.Installing:
                case ARSessionState.NeedsInstall:
                    break;
                case ARSessionState.None:
                    throw new InvalidOperationException("Cannot install until availability has been determined. Have you called CheckAvailability()?");
                case ARSessionState.Ready:
                case ARSessionState.SessionInitializing:
                case ARSessionState.SessionTracking:
                    yield break;
                case ARSessionState.Unsupported:
                    throw new InvalidOperationException("Cannot install because XR is not supported on this platform.");
            }
            var subsystem = GetSubsystem();
            if (subsystem == null)
                throw new InvalidOperationException("The subsystem was destroyed while attempting to install AR software.");
            state = ARSessionState.Installing;
            var installPromise = subsystem.InstallAsync();
            yield return installPromise;
            var installStatus = installPromise.result;
            switch (installStatus)
            {
                case SessionInstallationStatus.Success:
                    state = ARSessionState.Ready;
                    s_Availability = (s_Availability | SessionAvailability.Installed);
                    break;
                case SessionInstallationStatus.ErrorUserDeclined:
                    state = ARSessionState.NeedsInstall;
                    break;
                default:
                    state = ARSessionState.Unsupported;
                    break;
            }
        }


        static XRSessionSubsystem GetSubsystem()
        {
            if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
            {
                var loader = XRGeneralSettings.Instance.Manager.activeLoader;
                if (loader != null)
                {
                    return loader.GetLoadedSubsystem<XRSessionSubsystem>();
                }
            }
            return null;
        }



        static SessionAvailability s_Availability;


        public void AcaoBotao()
        {
            SceneManager.LoadScene(1);
        }
    }
}



/*
public class SceneSplashJMF : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadScene(1);
        }
    }
*/