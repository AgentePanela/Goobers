// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 c4llv07e <38111072+c4llv07e@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;
using Content.Client.Access.UI;
using Content.Shared.Access;
using Content.Shared.Doors.Electronics;
using FancyWindow = Content.Client.UserInterface.Controls.FancyWindow;

namespace Content.Client.Doors.Electronics;

[GenerateTypedNameReferences]
public sealed partial class DoorElectronicsConfigurationMenu : FancyWindow
{
    private readonly AccessLevelControl _buttonsList = new();

    public event Action<List<ProtoId<AccessLevelPrototype>>>? OnAccessChanged;

    public DoorElectronicsConfigurationMenu()
    {
        RobustXamlLoader.Load(this);
        AccessLevelControlContainer.AddChild(_buttonsList);
    }

    public void Reset(IPrototypeManager protoManager, List<ProtoId<AccessLevelPrototype>> accessLevels)
    {
        _buttonsList.Populate(accessLevels, protoManager);

        foreach (var button in _buttonsList.ButtonsList.Values)
        {
            button.OnPressed += _ => OnAccessChanged?.Invoke(_buttonsList.ButtonsList.Where(x => x.Value.Pressed).Select(x => x.Key).ToList());
        }
    }

    public void UpdateState(DoorElectronicsConfigurationState state)
    {
        _buttonsList.UpdateState(state.AccessList);
    }
}