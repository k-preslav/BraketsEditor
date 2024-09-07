using BraketsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace BraketsPluginIntegration
{
    public interface IEditorPlugin
    {
        DebugWindow Window { get; set; }

        void Initialize();
        void Draw();
        void Update();
    }
}
