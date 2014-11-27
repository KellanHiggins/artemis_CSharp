#region File description

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="GAMADU.COM">
//     Copyright © 2013 GAMADU.COM. All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without modification, are
//     permitted provided that the following conditions are met:
//
//        1. Redistributions of source code must retain the above copyright notice, this list of
//           conditions and the following disclaimer.
//
//        2. Redistributions in binary form must reproduce the above copyright notice, this list
//           of conditions and the following disclaimer in the documentation and/or other materials
//           provided with the distribution.
//
//     THIS SOFTWARE IS PROVIDED BY GAMADU.COM 'AS IS' AND ANY EXPRESS OR IMPLIED
//     WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//     FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL GAMADU.COM OR
//     CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//     CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//     SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//     ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//     NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
//     ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     The views and conclusions contained in the software and documentation are those of the
//     authors and should not be interpreted as representing official policies, either expressed
//     or implied, of GAMADU.COM.
// </copyright>
// <summary>
//   Basic unity of this entity system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
#endregion File description

namespace Artemis
{
    #region Using statements

    using global::System.Diagnostics;

#if !XBOX && !WINDOWS_PHONE  && !PORTABLE
    using global::System.Numerics;
#endif
#if XBOX || WINDOWS_PHONE || PORTABLE || FORCEINT32
    using BigInteger = global::System.Int32;
#endif
    using Artemis.Interface;
    using Artemis.Manager;
    using Artemis.Utils;

    #endregion

    /// <summary>Basic unity of this entity system.</summary>
    public sealed class Entity
    {
        /// <summary>The entity manager.</summary>
        private readonly EntityManager entityManager;

        /// <summary>The entity world.</summary>
        private readonly EntityWorld entityWorld;

        /// <summary> A list of all present components in the entity. Used to quickly retrieve all the components in an entity</summary>
        private Bag<IComponent> entityComponents;

        /// <summary>
        /// The unique id.
        /// This ID is unique in Artemis (even if the Entity is reused)
        /// This value can be SET when building the Entity (passed as a parameter to EntityWorld.CreateEntity)
        /// </summary>
        private long uniqueId;

        /// <summary>Initializes a new instance of the <see cref="Entity"/> class.</summary>
        /// <param name="entityWorld">The entity world.</param>
        /// <param name="id">The id.</param>
        internal Entity(EntityWorld entityWorld, int id)
        {
            this.SystemBits = 0;
            this.TypeBits = 0;
            this.IsEnabled = true;
            this.entityWorld = entityWorld;
            this.entityManager = entityWorld.EntityManager;
            this.Id = id;
            this.entityComponents = new Bag<IComponent>();
        }

        /// <summary>
        ///   <para>Gets all components belonging to this entity.</para>
        ///   <para>The components are stored within a private variable that is updated</para>
        ///   <para>each time the entity has components added or removed</para>
        /// </summary>
        /// <value>All components of this entity.</value>
        public Bag<IComponent> Components
        {
            get
            {
                return this.entityComponents;
            }
            internal set
            {
                this.entityComponents = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether [deleting state].</summary>
        /// <value><see langword="true" /> if [deleting state]; otherwise, <see langword="false" />.</value>
        public bool DeletingState { get; set; }

        /// <summary>Gets or sets a value indicating whether this <see cref="Entity" /> is enabled.</summary>
        /// <value><see langword="true" /> if enabled; otherwise, <see langword="false" />.</value>
        public bool IsEnabled { get; set; }

        /// <summary>Gets or sets the group.</summary>
        /// <value>The group.</value>
        public string Group
        {
            get
            {
                return this.entityWorld.GroupManager.GetGroupOf(this);
            }

            set
            {
                this.entityWorld.GroupManager.Set(value, this);
            }
        }

        /// <summary>
        /// <para>Gets the internal id for this entity within the framework.</para>
        /// <para>No other entity will have the same ID,</para>
        /// <para>but IDs are however reused so another entity may acquire</para>
        /// <para>this ID if the previous entity was deleted.</para>
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; private set; }

        /// <summary>Gets or sets a value indicating whether [refreshing state].</summary>
        /// <value><see langword="true" /> if [refreshing state]; otherwise, <see langword="false" />.</value>
        public bool RefreshingState { get; set; }

        /// <summary>Gets or sets the tag.</summary>
        /// <value>The tag.</value>
        public string Tag
        {
            get
            {
                return this.entityWorld.TagManager.GetTagOfEntity(this);
            }

            set
            {
                var oldTag = this.entityWorld.TagManager.GetTagOfEntity(this);
                if (value != oldTag)
                {
                    if(oldTag != null)
                        this.entityWorld.TagManager.Unregister(this);

                    if(value != null)
                        this.entityWorld.TagManager.Register(value, this);
                }
            }
        }

        /// <summary>Gets the unique id.</summary>
        /// <value>The unique id.</value>
        public long UniqueId
        {
            get
            {
                return this.uniqueId;
            }

            internal set
            {
                Debug.Assert(this.uniqueId >= 0, "UniqueId must be at least 0.");

                this.uniqueId = value;
            }
        }

        /// <summary>Gets a value indicating whether this instance is active.</summary>
        /// <value><see langword="true" /> if this instance is active; otherwise, <see langword="false" />.</value>
        public bool IsActive
        {
            get
            {
                return this.entityManager.IsActive(this.Id);
            }
        }

        /// <summary>Gets or sets the system bits.</summary>
        /// <value>The system bits.</value>
        internal BigInteger SystemBits { get; set; }

        /// <summary>Gets or sets the type bits.</summary>
        /// <value>The type bits.</value>
        internal BigInteger TypeBits { get; set; }

        /// <summary>Adds the component.</summary>
        /// <param name="component">The component.</param>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component must not be null.");

            this.entityManager.AddComponent(this, component);
        }

        /// <summary>Adds the component.</summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <param name="component">The component.</param>
        public void AddComponent<T>(T component) where T : IComponent
        {
            Debug.Assert(component != null, "Component must not be null.");

            this.entityManager.AddComponent<T>(this, component);
        }

        /// <summary>Adds the component from pool.</summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <returns>The added component.</returns>
        public T AddComponentFromPool<T>() where T : ComponentPoolable
        {
            T component = this.entityWorld.GetComponentFromPool<T>();
            this.entityManager.AddComponent<T>(this, component);
            return component;
        }

        /// <summary>Gets the component from pool, runs init delegate, then adds the components to the entity.</summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <returns>The added component.</returns>
        public void AddComponentFromPool<T>(global::System.Action<T> init) where T : ComponentPoolable
        {
            Debug.Assert(init != null, "Init delegate must not be null.");

            T component = this.entityWorld.GetComponentFromPool<T>();
            init(component);
            this.entityManager.AddComponent<T>(this, component);
        }

        /// <summary>Deletes this instance.</summary>
        public void Delete()
        {
            if (this.DeletingState)
            {
                return;
            }

            this.entityWorld.DeleteEntity(this);
            this.ClearComponents(); // clears the fake component bag because we know this will be deleted next update
            this.DeletingState = true;
        }

        /// <summary>
        /// <para>Gets the component.</para>
        /// <para>Slower retrieval of components from this entity.</para>
        /// <para>Minimize usage of this, but is fine to use e.g. when</para>
        /// <para>creating new entities and setting data in components.</para>
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns>component that matches, or null if none is found.</returns>
        public IComponent GetComponent(ComponentType componentType)
        {
            Debug.Assert(componentType != null, "Component type must not be null.");

            return this.entityManager.GetComponent(this, componentType);
        }

        /// <summary>
        /// <para>Gets the component.</para>
        /// <para>This is the preferred method to use when</para>
        /// <para>retrieving a component from a entity.</para>
        /// <para>It will provide good performance.</para>
        /// </summary>
        /// <typeparam name="T">the expected return component type.</typeparam>
        /// <returns>component that matches, or null if none is found.</returns>
        public T GetComponent<T>() where T : IComponent
        {
            return (T)this.entityManager.GetComponent(this, ComponentType<T>.CType);
        }

        /// <summary>Determines whether this instance has a specific component.</summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <returns><see langword="true" /> if this instance has a specific component; otherwise, <see langword="false" />.</returns>
        public bool HasComponent<T>() where T : IComponent
        {
            return !object.Equals((T)this.entityManager.GetComponent(this, ComponentType<T>.CType), default(T));
        }

        /// <summary><para>Refreshes this instance.</para>
        ///   <para>Refresh all changes to components for this entity.</para>
        ///   <para>After adding or removing components,</para>
        ///   <para>you must call this method.</para>
        ///   <para>It will update all relevant systems.</para>
        ///   <para>It is typical to call this after adding components</para>
        ///   <para>to a newly created entity.</para>
        /// </summary>
        public void Refresh()
        {
            if (this.RefreshingState)
            {
                return;
            }

            this.entityWorld.RefreshEntity(this);
            
            this.RefreshingState = true;
        }

        /// <summary>Remove Component from this entity.</summary>
        /// <typeparam name="T">Component Type.</typeparam>
        public void RemoveComponent<T>() where T : IComponent
        {
            this.entityManager.RemoveComponent(this, ComponentTypeManager.GetTypeFor<T>());
        }

        /// <summary><para>Removes the component.</para>
        ///   <para>Faster removal of components from a entity.</para></summary>
        /// <param name="componentType">The type.</param>
        public void RemoveComponent(ComponentType componentType)
        {
            Debug.Assert(componentType != null, "Component type must not be null.");

            this.entityManager.RemoveComponent(this, componentType);
        }

        /// <summary>
        /// Refreshes the store bag of components for this entity
        /// </summary>
        public void RefreshComponentBag()
        {
            entityComponents = this.entityManager.GetComponents(this);
        }

        /// <summary>
        /// Removes all components from this entity, reducing component count to zero
        /// </summary>
        public void RemoveAllComponents()
        {
            this.entityWorld.EntityManager.RemoveComponentsOfEntity(this);
        }

        /// <summary>
        /// Clears the bag with all the components in it;
        /// </summary>
        internal void ClearComponents()
        {
            entityComponents.Clear();
        }

        /// <summary>Resets the entity to default and clears all components.</summary>
        internal void Reset()
        {
            this.SystemBits = 0;
            this.TypeBits = 0;
            this.IsEnabled = true;
            this.ClearComponents(); 
        }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("Entity{{{0}}}", this.Id);
        }

        /// <summary>Adds the system bit.</summary>
        /// <param name="bit">The bit.</param>
        internal void AddSystemBit(BigInteger bit)
        {
            this.SystemBits |= bit;
        }

        /// <summary>Adds the type bit.</summary>
        /// <param name="bit">The bit.</param>
        internal void AddTypeBit(BigInteger bit)
        {
            this.TypeBits |= bit;
        }

        /// <summary>Removes the system bit.</summary>
        /// <param name="bit">The bit.</param>
        internal void RemoveSystemBit(BigInteger bit)
        {
            this.SystemBits &= ~bit;
        }

        /// <summary>Removes the type bit.</summary>
        /// <param name="bit">The bit.</param>
        internal void RemoveTypeBit(BigInteger bit)
        {
            this.TypeBits &= ~bit;
        }


    }
}