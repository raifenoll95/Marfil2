﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marfil.Inf.ResourcesGlobalization.Textos.Entidades {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Gruposusuarios {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Gruposusuarios() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Gruposusuarios", typeof(Gruposusuarios).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ya existe el grupo.
        /// </summary>
        public static string ErrorGrupoExistente {
            get {
                return ResourceManager.GetString("ErrorGrupoExistente", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to El usuario {0} está duplicado.
        /// </summary>
        public static string ErrorUsuarioExistente {
            get {
                return ResourceManager.GetString("ErrorUsuarioExistente", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Permisos.
        /// </summary>
        public static string LblPermisos {
            get {
                return ResourceManager.GetString("LblPermisos", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Grupo.
        /// </summary>
        public static string Role {
            get {
                return ResourceManager.GetString("Role", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Grupos.
        /// </summary>
        public static string TituloEntidad {
            get {
                return ResourceManager.GetString("TituloEntidad", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Usuario.
        /// </summary>
        public static string Usuarios {
            get {
                return ResourceManager.GetString("Usuarios", resourceCulture);
            }
        }
    }
}
