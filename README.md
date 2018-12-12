READ ME
=======
These are the commands to control the tree.


**NOTE:** Remember to have multiple copies of the client open and connected before loading in the tree text.

## Loading Tree From File

`path **filePath**`

This is the command to make a tree from a text file
```
path C:\desktop\file.txt
```

## Add New Node To Tree

`add **parentID**,**nameOfNewNode**`

This is the command to a new node to the tree. To make a root node, replace **parentID** with **null**.
```
add 5,Jonathan //adds a new node to an existing node
add null, Juliet //adds a new root node
```

## Remove A Node From File

`remove **nodeID**`

This is a command to remove a node from the tree.
```
remove 7
``` 

## Move A Node To A New Parent

`move **nodeToBeMovedID**,**newParentID**`

This is the command to move one node to a new parent.
```
move 5,8
```

## Get A Node

### By ID

`get **nodeID**`

This is the command to get a node by ID.
```
get 9
```
### By Name

`get **nodeName**`
This is the command to get a node by name.
```
get Georgia
```
### Leaves

`get leaves`

This is the command to get all leaf nodes.
```
get leaves
```
### Internal Nodes

`get internal`

This is the command to get all internal nodes.
```
get internal
```

## Read Tree

`read`

This is the command to read the names and IDs of all of the nodes in the tree in hierarchical format.
`read`
