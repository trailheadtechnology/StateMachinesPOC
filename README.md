# StateMachinesPOC
 Use Stateless library to write Finite State Machines

## Resources
### State Machine theory:
- https://www.itemis.com/en/yakindu/state-machine/documentation/user-guide/overview_what_are_state_machines
- https://en.wikipedia.org/wiki/Finite-state_machine
### Stateless library: https://github.com/dotnet-state-machine/stateless
### XamBoy example: https://www.xamboy.com/2021/08/17/using-state-machine-in-xamarin-forms-part-1/ 
- [Look also for part 2 and 3 in the same blog](https://www.xamboy.com/category/netlibraries/stateless/)

### Visualization
Trying to use this tools for visualization are hard because the dot graph visualization doesn't really support some of the stateless features and will render in a wrong way
#### Visualize graphs online: https://edotor.net/
#### Dot reference: https://www.graphviz.org/pdf/dotguide.pdf
```dot
digraph{
compound=true;
node[shape=Mrecord]
rankdir="LR"

"NotSet"->"NotFoundFromServer"[style="solid",label="LoadFromServerNotFound"];
"NotSet"->"LoadingFromServer"[style="solid",label="LoadFromServer"];
"NotFoundFromServer"->"LoadingLocally"[style="solid",label="LoadLocally"];
"LoadingFromServer"->"Loaded"[style="solid",label="CompleteLoad"];
"LoadingLocally"->"Loaded"[style="solid",label="CompleteLoad"];
"CreatedLocally"->"Loaded"[style="solid",label="CompleteLoad"];
"LoadingLocally"->"NotFoundFromLocal"[style="solid",label="LoadLocallyNotFound"];
"NotFoundFromLocal"->"CreatedLocally"[style="solid",label="CreateLocally"];
"Loaded"->"Changed"[style="solid",label="ChangeValues"];
"Changed"->"Changed"[style="solid",label="ChangeValues"];
"Saved"->"Changed"[style="solid",label="ChangeValues"];
"Saved"->"NotSet"[style="solid",label="Close"];
"Loaded"->"NotSet"[style="solid",label="Close"];
"Changed"->"Saved"[style="solid",label="Save"];
"Changed"->"NotSet"[style="solid",label="Discard"];
init[label="",shape=point];
init->"NotSet"[style="solid"]
}
```