using Newtonsoft.Json.Linq;
using ReactNative.UIManager;
using ReactNative.UIManager.Annotations;
using ReactNative.UIManager.Events;
using System;

using ElmSharp;
using ReactNativeTizen.ElmSharp.Extension;

namespace ReactNative.Views.Switch
{
    /// <summary>
    /// A view manager for the <see cref="ToggleSwitch"/> element.
    /// </summary>
    public class ReactSwitchManager : BaseViewManager<Check, ReactSwitchShadowNode>
    {
        private const string ReactClass = "RCTSwitch";

        /// <summary>
        /// The name of this view manager. This will be the name used to 
        /// reference this view manager from JavaScript.
        /// </summary>
        public override string Name
        {
            get
            {
                return ReactClass;
            }
        }

        /// <summary>
        /// Sets whether a toggle switch is enabled.
        /// </summary>
        /// <param name="view">The toggle switch view.</param>
        /// <param name="disabled">
        /// Set to <code>true</code> if the toggle switch should be enabled,
        /// otherwise, set to <code>false</code>.
        /// </param>
        [ReactProp(ViewProps.Disabled)]
        public void SetDisabled(Check view, bool disabled)
        {
            view.IsEnabled = !disabled;
        }

        /// <summary>
        /// Sets whether a toggle switch is currently toggled.
        /// </summary>
        /// <param name="view">The toggle switch view.</param>
        /// <param name="on">
        /// Set to <code>true</code> if the toggle switch is toggled on,
        /// otherwise, set to <code>false</code>.
        /// </param>
        [ReactProp(ViewProps.Value)]
        public void SetValue(Check view, bool on)
        {
            // Temporarily disable toggle event handler.
            view.StateChanged -= OnToggled;
            view.IsChecked = on;
            view.StateChanged += OnToggled;
        }

        /// <summary>
        /// This method should return the <see cref="ReactSwitchShadowNode"/>
        /// which will be then used for measuring the position and size of the
        /// view. 
        /// </summary>
        /// <returns>The shadow node instance.</returns>
        public override ReactSwitchShadowNode CreateShadowNodeInstance()
        {
            return new ReactSwitchShadowNode();
        }

        /// <summary>
        /// Implement this method to receive optional extra data enqueued from
        /// the corresponding instance of <see cref="ReactShadowNode"/> in
        /// <see cref="ReactShadowNode.OnCollectExtraUpdates"/>.
        /// </summary>
        /// <param name="root">The root view.</param>
        /// <param name="extraData">The extra data.</param>
        public override void UpdateExtraData(Check root, object extraData)
        {
        }

        /// <summary>
        /// Called when view is detached from view hierarchy and allows for 
        /// additional cleanup by the <see cref="ReactSwitchManager"/>.
        /// </summary>
        /// <param name="reactContext">The React context.</param>
        /// <param name="view">The view.</param>
        public override void OnDropViewInstance(ThemedReactContext reactContext, Check view)
        {
            base.OnDropViewInstance(reactContext, view);
            view.StateChanged -= OnToggled;
        }

        /// <summary>
        /// Creates a new view instance of type <see cref="ToggleSwitch"/>.
        /// </summary>
        /// <param name="reactContext">The React context.</param>
        /// <returns>The view instance.</returns>
        protected override Check CreateViewInstance(ThemedReactContext reactContext)
        {
            var view = new Check(ReactProgram.RctWindow)
            {
                Style = "toggle",
            };

            view.Show();
            //view.OnContent = null;
            //view.OffContent = null;
            return view;
        }

        /// <summary>
        /// Subclasses can override this method to install custom event 
        /// emitters on the given view.
        /// </summary>
        /// <param name="reactContext">The React context.</param>
        /// <param name="view">The view instance.</param>
        protected override void AddEventEmitters(ThemedReactContext reactContext, Check view)
        {
            base.AddEventEmitters(reactContext, view);
            view.StateChanged += OnToggled;
        }

        private void OnToggled(object sender, CheckStateChangedEventArgs e)
        {
            var toggleSwitch = (Check)sender;
            var reactContext = toggleSwitch.GetReactContext();
            reactContext.GetNativeModule<UIManagerModule>()
                .EventDispatcher
                .DispatchEvent(
                    new ReactSwitchEvent(
                        toggleSwitch.GetTag(),
                        toggleSwitch.IsChecked));
        }

        class ReactSwitchEvent : Event
        {
            private readonly bool _isChecked;

            public ReactSwitchEvent(int viewTag, bool isChecked)
                : base(viewTag, TimeSpan.FromTicks(Environment.TickCount))
            {
                _isChecked = isChecked;
            }

            public override string EventName
            {
                get
                {
                    return "topChange";
                }
            }

            public override void Dispatch(RCTEventEmitter eventEmitter)
            {
                var eventData = new JObject
                {
                    { "target", ViewTag },
                    { "value", _isChecked },
                };

                eventEmitter.receiveEvent(ViewTag, EventName, eventData);
            }
        }
    }
}