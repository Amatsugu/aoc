#!/usr/bin/env python3
from networkx  import DiGraph, lexicographical_topological_sort as lts

# Step T must be finished before step P can begin.
# Step Q must be finished before step W can begin.


def main():
    with open('sinput', 'r') as fp:
    #with open('input', 'r') as fp:
        depTree = DiGraph([(x[1], x[7]) for x in map(str.split, fp.readlines())])

    part1 = lts(depTree)
    print()
    print(''.join(part1))

    t = 0

    node_list = depTree.nodes()
    queue = [None for _ in range(5)]

    while node_list:
        for 

if __name__ == '__main__':
    main()
