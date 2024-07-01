# Handling Input

In your mod, you may want to react to user input. For example, you might want to perform an action when the player presses a button. To do that, we can use the `HandleInput` method.

## HandleInput

This method is called every frame and is used for handling user input, such as button presses. This method should return true if any input was handled, otherwise return false.

```csharp
public bool HandleInput(ITMPlayer player)
{
    // Called every frame.
    // React to user input here.
    // This method should return true if it does something in response
    // to user input, otherwise false.
    return false;
}
```

Let's launch the player into the air when they're on the ground and press the `R` key.

To check if the player is pressing a key, we can use `InputManager` from `StudioForge.Engine.Core`. This class is static and has a few useful methods that we're interested in:
- `IsKeyPressed`
  - This returns true if the player is holding a key. From the moment they begin holding the key to the moment they stop holding the key, this will always return true. This is useful when you want to do something every frame while the player holds a key.
- `IsKeyPressedNew`
  - This returns true the first frame the player presses a key. This is useful if we only want to react to the input once.

Because we're triggering a single action on a key press, we'll use `IsKeyPressedNew`.

Both of these methods take a `PlayerIndex` for the player to test the input for, and the key to test. The `PlayerIndex` is available through `ITMPlayer.PlayerIndex` and represents the player when multiple local players exist. On PC, this isn't possible, but on Xbox 360, this would be different for each player in splitscreen.

**NOTE:** Even though it isn't possible to have multiple local players on PC, you should still use `ITMPlayer.PlayerIndex` instead of hardcoding the PlayerIndex in case that functionality is added back.

To launch the player into the air, we'll set their Y velocity to a positive value when the press `R` while on the ground:

```csharp
public bool HandleInput(ITMPlayer player)
{
    // Called every frame.
    // React to user input here.
    // This method should return true if it does something in response
    // to user input, otherwise false.

    // We launch the player into the air when they're on the ground
    // and press R.
    if (player.IsOnGround && InputManager.IsKeyPressed(player.PlayerIndex, Keys.R))
    {
        // Be careful with high velocities - this number is the number
        // of blocks the player moves every frame, not every second!
        // Because Total Miner is locked to 60 FPS, By dividing our
        // target blocks per second by 60, we get the correct value.
        // 20f / 60f gives a velocity of 20 blocks/s.
        player.Velocity = new Vector3(player.Velocity.X, 20f / 60f, player.Velocity.Z);
        return true;
    }

    return false;
}
```

**NOTE:** The player's velocity is measured in blocks per *frame*, not blocks per *second*. Total Miner is locked to 60 FPS, so to convert blocks per second to blocks per frame, we can just divide the number by 60.

*Make sure to use floats for the division, or you may get an unexpected result!*

We also return true immediately after launching the player. This works because we only have a single input, but if we had multiple inputs, we'd instead want to store the result in a variable and return that variable after all inputs have been checked. Otherwise, our return statement would prevent our other inputs from occurring.

## Handling controller input

Total Miner also has full controller support. Currently, our code only works for keyboard. If we want to support controller, we'll have to use different methods on `InputManager`:
- `IsButtonPressed`
  - Just like `IsKeyPressed`, this method returns true while the specified button is held. Instead of a keyboard key, this method takes a controller button.
- `IsButtonPressedNew`
  - Just like `IsKeyPressedNew`, this method returns true during the first frame the specified button is pressed. Instead of a keyboard key, this method takes a controller button.

**NOTE:** `Buttons` defines buttons for an Xbox controller! ie. A, B, X, Y

Before we add controller support, let's put our launch functionality into its own method so we don't have to change it in every place we use it:

```csharp
public bool HandleInput(ITMPlayer player)
{
    // Called every frame.
    // React to user input here.
    // This method should return true if it does something in response
    // to user input, otherwise false.

    // We launch the player into the air when they're on the ground
    // and press R.
    if (player.IsOnGround && InputManager.IsKeyPressed(player.PlayerIndex, Keys.R))
    {
        LaunchPlayer(player);
        return true;
    }

    return false;
}

private void LaunchPlayer(ITMPlayer player)
{
    // Be careful with high velocities - this number is the number
    // of blocks the player moves every frame, not every second!
    // Because Total Miner is locked to 60 FPS, By dividing our
    // target blocks per second by 60, we get the correct value.
    // 20f / 60f gives a velocity of 20 blocks/s.
    player.Velocity = new Vector3(player.Velocity.X, 20f / 60f, player.Velocity.Z);
}
```

Due to controllers having fewer buttons, we'll have to get creative with the input. While not the best for actual gameplay, I'll bind our jump to DPad Down + A.

```csharp
public bool HandleInput(ITMPlayer player)
{
    // Called every frame.
    // React to user input here.
    // This method should return true if it does something in response
    // to user input, otherwise false.

    // We launch the player into the air when they're on the ground
    // and press R on keyboard, or DPad Down + A on controller.
    if (player.IsOnGround)
    {
        // Keyboard input
        if (InputManager.IsKeyPressed(player.PlayerIndex, Keys.R))
        {
            LaunchPlayer(player);
            return true;
        }
        // Controller input
        else if (InputManager.IsButtonPressed(player.PlayerIndex, Buttons.DPadDown)
            && InputManager.IsButtonPressed(player.PlayerIndex, Buttons.A))
        {
            LaunchPlayer(player);
            return true;
        }
    }

    return false;
}
```

There is another way to handle both keyboard and controller input, and that is through `InputManager1`. This class is also static and defines a few methods we'll want: `IsInputPressed` and `IsInputPressedNew`. These function exactly the same as the previous methods, but take a `PlayerInput` instead of a key or button. A `PlayerInput` is some input action, such as swinging or jumping, and can be rebound in the options menu.

By using this class, we can react to both keyboard and controller using an input, but sometimes it isn't as useful because many of the inputs are already bound to different actions. Since it's otherwise identical to the methods we've already uses, I'll leave that one for you to try.