﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImageMap4.CMD.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ImageMap4.CMD.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to bedrock:
        ///- name: &quot;&quot;
        ///  check:
        ///    version: [1,2]
        ///  map_data:
        ///    mapId: &apos;@id&apos;
        ///    parentMapId: -1L
        ///    colors: &apos;@colors&apos;
        ///    mapLocked: 1b
        ///    scale: 4b
        ///    dimension: 0b
        ///    fullyExplored: 1b
        ///    unlimitedTracking: 0b
        ///    xCenter: 2147483647
        ///    zCenter: 2147483647
        ///    height: 128s
        ///    width: 128s
        ///
        ///java:
        ///- name: Beta 1.8
        ///  check:
        ///    path: MapFeatures
        ///  multipliers: [180, 220, 255, 220]
        ///  set_base_colors:
        ///  - transparent
        ///  - 7fb238
        ///  - f7e9a3
        ///  - a7a7a7
        ///  - ff0000
        ///  - a0a0ff
        ///   [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string versions {
            get {
                return ResourceManager.GetString("versions", resourceCulture);
            }
        }
    }
}
