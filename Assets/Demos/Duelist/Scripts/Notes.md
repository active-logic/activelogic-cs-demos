# Stuff that can be simplified...

- Assets, such as animation controllers or sound maps, can be inferred from classes. This means that we write only controller code. Inference can happen statically, but it can also be done at runtime, which 'catches' non literal animation/sound calls.

- Perception is about filtering. An agent has a number of 'objects of interest' and this can be expressed using a combination of filters (think &|).

- As we can infer animation controllers from code assets, we can also infer control class bases from animation/model data.

- Then think of a way to concisely express random generators
