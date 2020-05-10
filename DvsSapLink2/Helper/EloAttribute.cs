using System;

namespace DvsSapLink2.Helper
{
    public class EloAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EloAttribute"/> class.
        /// </summary>
        public EloAttribute(string description, int order)
        {
            this.Description = description;
            this.Order = order;
        }
        
        /// <summary>
        /// Gets the description of the attribute
        /// </summary>
        public string Description { get; }
       
        /// <summary>
        /// Gets the order of the attribute
        /// </summary>
        public int Order { get; }
    }
}